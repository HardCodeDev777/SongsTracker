using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;
using Telegram.Bot;

namespace SongsTracker.MusicRequests;

public class SongsTrackWorker(IServiceScopeFactory scopeFactory, 
    ITelegramBotClient bot, ILogger<SongsTrackWorker> logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        await CheckSongs(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            await CheckSongs(stoppingToken);
    }

    private async Task CheckSongs(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();
        var musicService = scope.ServiceProvider.GetRequiredService<IMusicService>();

        var artistsToCheck = await dbContext.UniqueArtists
            .Include(a => a.Followers)
            .ToListAsync(stoppingToken);

        logger.LogInformation($"Artists amount to check: {artistsToCheck.Count}");
        foreach (var artist in artistsToCheck)
        {
            var artistLatestSong = await musicService
                .GetLatestSongShortData(artist.Id, stoppingToken);

            if (artistLatestSong is not null 
                && artistLatestSong.ReleaseDate > artist.LastReleaseDate)
            {
                artist.LastReleaseDate = artistLatestSong.ReleaseDate.ToUniversalTime();
                logger.LogInformation($"{artist.Name} dropped new song: {artistLatestSong.Title} \n" +
                    $"Date: {artistLatestSong.ReleaseDate}");

                var usersToSend = artist.Followers;
                foreach (var user in usersToSend)
                    await bot.SendMessage(chatId: user.Id, $"{artist.Name} dropped new song: {artistLatestSong.Title} \n" +
                    $"Date: {artistLatestSong.ReleaseDate}", 
                        cancellationToken: stoppingToken);             
            }

            // To avoid blocking
            await Task.Delay(2000, stoppingToken);
        }

        await dbContext.SaveChangesAsync(stoppingToken);
        logger.LogInformation($"{DateTime.Now}: check completed");
    }
}
