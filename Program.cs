using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Data.Sqlite;

namespace ReactionRolesBotCS {
    class Program {
        static async Task Main(string[] args) {

            //
            // Initialize Database

            using (var Connection = new SqliteConnection("Data Source=database.db")) {
                Connection.Open();
                var command = Connection.CreateCommand();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS `reactionRoles` ( `message` INTEGER NOT NULL, `emoji` TEXT NOT NULL, `role` INTEGER NOT NULL );";
                command.ExecuteNonQuery();
            }

            //
            // Setup Client

            string Token = File.ReadAllText("token.txt");

            DiscordConfiguration ClientConfig = new DiscordConfiguration() {
                Token = Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.GuildMessageReactions |
                DiscordIntents.AllUnprivileged
            };

            DiscordClient discordClient = new DiscordClient(ClientConfig);

            //
            // Register Slash Commands

            discordClient.UseSlashCommands().RegisterCommands<ReroSlashCommands>(969479656069804063);

            //
            // Bind Event Handlers

            //
            // Launch Client
            
            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}