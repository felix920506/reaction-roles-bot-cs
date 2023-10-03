using System.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Data.Sqlite;

namespace ReactionRolesBotCS;

public class ReactionRoleHandler
{
	public static async Task ReactionEventHandler(DiscordClient client, MessageReactionAddEventArgs eventArgs)
	{
		//
		// Filter out DMs

		if (eventArgs.Guild == null)
		{
			return;
		}

		//
		// Query database

		ulong roleId = 0;

		await using (var connection = new SqliteConnection("Data Source=database.db"))
		{
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = "SELECT role FROM reactionRoles WHERE message = $message AND emoji = $emoji LIMIT 1";
			command.Parameters.AddWithValue("$message", eventArgs.Message.Id);
			command.Parameters.AddWithValue("$emoji", eventArgs.Emoji.ToString());

			await using (var reader = await command.ExecuteReaderAsync())
			{
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						if (reader.IsDBNull(0)) continue;
						roleId = (ulong)reader.GetInt64("role");
						break;
					}
				}
			}
		}

		//
		// Exit if nothing was found

		if (roleId == 0)
		{
			return;
		}

		//
		// Grant role to user

		var targetRole = eventArgs.Guild.GetRole(roleId);

		var member = (DiscordMember)eventArgs.User;
		await member.GrantRoleAsync(targetRole);
	}
}