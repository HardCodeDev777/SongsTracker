using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;
using SongsTracker.Data.Models;
using SongsTracker.MusicRequests;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SongsTracker.Bot.Commands;

public class AddArtistCommand(IMusicService musicService, 
    TrackerDbContext dbContext, ILogger<AddArtistCommand> logger) : IBotCommand
{
    public string CommandName => "/add_artist";

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
        }

        var isAlreadyTracking = dbUser.TrackedArtists
            .Any(a => a.Name.Equals(artistName, StringComparison.OrdinalIgnoreCase));

        // If it's already tracking then it's added to UniqueArtists
        if (isAlreadyTracking)
        {
            await bot.SendMessage(message.Chat, "You already track this artist!", 
                cancellationToken: cancellationToken);
            return;
        }

        var dbArtist = await dbContext.UniqueArtists
            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.ToLower(), cancellationToken);

        if (dbArtist is null)
        {
            logger.LogInformation("No such artist in db");

            var responseIdAndName = await musicService
                .GetArtistIdAndFullnameByName(artistName, cancellationToken);

            if (responseIdAndName is null)
            {
                logger.LogError("Service response is null");
                return;
            }

            logger.LogInformation($"Added artist name: {responseIdAndName.Id}, " +
                $"artist id: {responseIdAndName.Name}");

            var responseLatestSong = await musicService
                .GetLatestSongShortData(responseIdAndName.Id, cancellationToken);

            dbArtist = new UniqueArtistsModel
            {
                Id = responseIdAndName.Id,
                Name = responseIdAndName.Name,
                LastReleaseDate = responseLatestSong!.ReleaseDate.ToUniversalTime() // For tests: new DateTime(2018, 2, 14)
            };
            dbContext.UniqueArtists.Add(dbArtist);

            await bot.SendMessage(message.Chat, $"Artist latest song: \n Name: {responseLatestSong.Title} " +
                $"\n Release date: {responseLatestSong.ReleaseDate}", cancellationToken: cancellationToken);
        }
        else
        {
            logger.LogInformation("Artist already exist");
        }

        dbUser.TrackedArtists.Add(dbArtist);
        dbArtist.Followers.Add(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        await bot.SendMessage(message.Chat, $"Artist {dbArtist.Name} added successfully!",
            cancellationToken: cancellationToken);
    }
}
