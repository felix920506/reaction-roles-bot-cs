using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

namespace ReactionRolesBotCS {
    [SlashCommandGroup("reactionroles", "Commands related to reaction roles")]

    public class ReroSlashCommands : ApplicationCommandModule {

        //
        // Add rero command

        [SlashCommand("add", "Adds a reaction role to your previous message in this channel")]
        public async Task addReroCommand(InteractionContext ctx, 
        [Option("emoji", "Reaction / Emoji for this reaction role")] string emoji,
        [Option("role", "Discord Role for this reaction")] DiscordRole role ) {

            //
            // Validate Permissions

            if (!ctx.Member.Permissions.HasPermission(Permissions.ManageRoles)) {

                DiscordInteractionResponseBuilder responseBuilder1 = new DiscordInteractionResponseBuilder();
                responseBuilder1.WithContent("You need manage roles permissions to use this command");
                responseBuilder1.AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder1);
                return;
            }

            //
            // Attempt to look for target message

            var messages = await ctx.Channel.GetMessagesAsync(100);
            DiscordMessage? targetMessage = null;

            for (int i = 0; i < messages.Count(); i++) {
                if (messages[i].Author.Id == ctx.Member.Id) {
                    targetMessage = messages[i];
                    break;
                }
            }

            if (targetMessage == null) {
                DiscordInteractionResponseBuilder responseBuilder2 = new DiscordInteractionResponseBuilder();
                responseBuilder2.WithContent("No messages from you were found within the last 100.");
                responseBuilder2.AsEphemeral();

                await ctx.CreateResponseAsync(responseBuilder2);
                return;
            }

            //
            // Attempt to add reaction to said message

            try {
                DiscordEmoji? targetEmoji = null;

                if (DiscordEmoji.IsValidUnicode(emoji)) {
                    targetEmoji = DiscordEmoji.FromUnicode(emoji);
                }
                else {
                    Regex emojiRegex = new Regex(@":[0-9]*>");
                    string match = emojiRegex.Matches(emoji)[0].Value;
                    ulong emojiId = UInt64.Parse(match.Substring(1, match.Length - 2));
                    targetEmoji = DiscordEmoji.FromGuildEmote(ctx.Client, emojiId);
                }

                await targetMessage.CreateReactionAsync(targetEmoji);
            }

            catch {

                DiscordInteractionResponseBuilder responseBuilder3 = new DiscordInteractionResponseBuilder();
                responseBuilder3.WithContent("The reaction is invalid or inaccessable to the Bot. Please use a default reaction or a reaction from this Discord server.");
                responseBuilder3.AsEphemeral();

                await ctx.CreateResponseAsync(responseBuilder3);
                return;
            }

            //
            // Save in database if success

            using (var connection = new SqliteConnection("Data Source=database.db")) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO reactionRoles (message, emoji, role) VALUES ($message, $emoji, $role)";
                command.Parameters.AddWithValue("$message", targetMessage.Id);
                command.Parameters.AddWithValue("$emoji", emoji);
                command.Parameters.AddWithValue("$role", role.Id);
                command.ExecuteNonQuery();
            }

            //
            // Send success message

            DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
            responseBuilder.WithContent("Reaction role registered.");
            responseBuilder.AsEphemeral();

            await ctx.CreateResponseAsync(responseBuilder);
            return;
        }
    }
}