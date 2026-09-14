using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;
using SongsTracker.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SongsTracker.Bot.Commands;

public class ListTrackedArtistsCommand(TrackerDbContext dbContext) : IBotCommand
{
    public string CommandName => "/list_tracked_artists";

    public async Task ExecuteAsync(ITelegramBotClient bot, Message message,
        CancellationToken cancellationToken)
    {
        var resultString = string.Empty;

        var chatId = message.Chat.Id;

        var dbUser = await dbContext.Users
            .Include(u => u.TrackedArtists)
            .FirstOrDefaultAsync(u => u.Id == chatId, cancellationToken);

        if (dbUser is null)
        {
            dbUser = new UserModel { Id = chatId, Name = message.From!.Username! };
            dbContext.Users.Add(dbUser);

            await bot.SendMessage(message.Chat, "You currently don't track any artist",
                cancellationToken: cancellationToken);
            return;
        }

        if (dbUser.TrackedArtists.Count == 0)
        {
            await bot.SendMessage(message.Chat, "You currently don't track any artist",
                cancellationToken: cancellationToken);
            return;
        }

        foreach (var artist in dbUser.TrackedArtists)
            resultString += $"{artist.Name} \n";

        await bot.SendMessage(message.Chat, $"Artists you track: \n{resultString}",
            cancellationToken: cancellationToken);
    }
}
