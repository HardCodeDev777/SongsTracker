using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;
using SongsTracker.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SongsTracker.Bot.Commands;

public class RemoveArtistCommand(TrackerDbContext dbContext, 
    ILogger<RemoveArtistCommand> logger) : IBotCommand
{
    public string CommandName => "/remove_artist";

    public async Task ExecuteAsync(ITelegramBotClient bot, Message message,
        CancellationToken cancellationToken)
    {
        var artistName = message.Text!.Replace(CommandName, "").Trim();
        logger.LogInformation($"Final artist name: {artistName}");

        if (string.IsNullOrEmpty(artistName))
        {
            await bot.SendMessage(message.Chat, "Command requires name of the artist",
                cancellationToken: cancellationToken);
            return;
        }

        var chatId = message.Chat.Id;

        var dbUser = await dbContext.Users
            .Include(u => u.TrackedArtists)
            .FirstOrDefaultAsync(u => u.Id == chatId, cancellationToken);

        if (dbUser is null)
        {
            dbUser = new UserModel { Id = chatId, Name = message.From!.Username! };
            dbContext.Users.Add(dbUser);

            await bot.SendMessage(message.Chat, "You currently don't track this artist",
             cancellationToken: cancellationToken);
            return;
        }

        if (dbUser.TrackedArtists.Count == 0)
        {
            await bot.SendMessage(message.Chat, "You currently don't track any artist",
                cancellationToken: cancellationToken);
            return;
        }

        var dbArtist = await dbContext.UniqueArtists
            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.ToLower(), cancellationToken);

        if (dbArtist is null)
        {
            await bot.SendMessage(message.Chat, "There's no artist like this",
                cancellationToken: cancellationToken);
            return;
        }

        dbUser.TrackedArtists.Remove(dbArtist);
        dbArtist.Followers.Remove(dbUser);

        if (dbArtist.Followers.Count == 0)
        {
            logger.LogInformation("Arist has no followers. Removing from db");
            dbContext.UniqueArtists.Remove(dbArtist);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);

        await bot.SendMessage(message.Chat, "Artist removed successfully",
            cancellationToken: cancellationToken);
    }
}
