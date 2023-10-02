using DSharpPlus;
using DSharpPlus.SlashCommands;



namespace ReactionRolesBotCS {
    class Program {
        static async Task Main(string[] args) {

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

            discordClient.UseSlashCommands().RegisterCommands<ReroSlashCommands>();

            //
            // Bind Event Handlers

            //
            // Launch Client
            
            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}