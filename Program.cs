using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Data.Sqlite;

namespace ReactionRolesBotCS;

internal class Program
{
	private static async Task Main(string[] args)
	{
		//
		// Initialize Database

		await using (var connection = new SqliteConnection("Data Source=database.db"))
		{
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText =
				@"CREATE TABLE IF NOT EXISTS `reactionRoles` ( `message` INTEGER NOT NULL, `emoji` TEXT NOT NULL, `role` INTEGER NOT NULL );";
			command.ExecuteNonQuery();
		}

		//
		// Setup Client

		var token = await File.ReadAllTextAsync("token.txt");

		var clientConfig = new DiscordConfiguration
		{
			Token = token,
			TokenType = TokenType.Bot,
			Intents = DiscordIntents.GuildMessageReactions |
			          DiscordIntents.AllUnprivileged
		};

		var discordClient = new DiscordClient(clientConfig);

		//
		// Register Slash Commands

		discordClient.UseSlashCommands().RegisterCommands<ReroSlashCommands>();

		//
		// Bind Event Handlers

		discordClient.MessageReactionAdded += ReactionRoleHandler.ReactionEventHandler;

		//
		// Launch Client

		await discordClient.ConnectAsync();
		await Task.Delay(-1);
	}
}