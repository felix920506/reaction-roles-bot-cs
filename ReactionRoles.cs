using System.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Data.Sqlite;

namespace ReactionRolesBotCS {
    public class ReactionRoleHandler {
        public static async Task ReactionEventHandler(DiscordClient client, MessageReactionAddEventArgs eventArgs) {
            
            //
            // Filter out DMs

            if (eventArgs.Guild == null) {
                return;
            }
            
            //
            // Query database

            ulong roleID = 0;

            using (var connection = new SqliteConnection("Data Source=database.db")) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT role FROM reactionRoles WHERE message = $message AND emoji = $emoji LIMIT 1";
                command.Parameters.AddWithValue("$message", eventArgs.Message.Id);
                command.Parameters.AddWithValue("$emoji", eventArgs.Emoji.ToString());
                
                using (var reader = command.ExecuteReader()) {
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            if (!reader.IsDBNull(0)) {
                                roleID = (ulong)reader.GetInt64("role");
                                break;
                            }
                        }
                    }
                }
            }

            //
            // Exit if nothing was found

            if (roleID == 0) {
                Console.WriteLine("test");
                return;
            }
            
            //
            // Grant role to user

            DiscordRole targetRole = eventArgs.Guild.GetRole(roleID);

            DiscordMember member = (DiscordMember)eventArgs.User;
            await member.GrantRoleAsync(targetRole);
            
        }
    }
}