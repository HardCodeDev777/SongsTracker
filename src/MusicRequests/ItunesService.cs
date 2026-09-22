using SongsTracker.Data.Models;

namespace SongsTracker.MusicRequests;

public class ItunesService(HttpClient httpClient, 
    ILogger<ItunesService> logger) : IMusicService
{
    public async Task<ArtistIdAndName?> GetArtistIdAndFullnameByName(string artistName,
        CancellationToken cancellationToken)
    {
        var url = 
            $"https://itunes.apple.com/search?term={Uri.EscapeDataString(artistName)}&entity=musicArtist&limit=1";
        
        logger.LogInformation($"Final url: {url}");
        var artistMainInfo = await httpClient
            .GetFromJsonAsync<ResponseRootobject>(url, cancellationToken);

        if (artistMainInfo is null) 
        {
            logger.LogError($"Couldn't parse json from url {url}");
            return null; 
        }

        if (artistMainInfo.resultCount == 0)
        {
            logger.LogError($"There's no artist with {artistName} name");
            return null;
        }
        
        var responseArtistName = artistMainInfo.results[0].artistName;
        var responseArtistId = artistMainInfo.results[0].artistId;

        return new(responseArtistId, responseArtistName);
    }

    public async Task<SongShortModel?> GetLatestSongShortData(long artistId, 
        CancellationToken cancellationToken)
    {
        // ITunes sucks, recent sort doesn't work.
        var url =
            $"https://itunes.apple.com/lookup?id={artistId}&entity=album&limit=200";
        logger.LogInformation($"Final url: {url}");

        var artistFullInfo = await httpClient
            .GetFromJsonAsync<ResponseRootobject>(url, cancellationToken);

        if (artistFullInfo is null)
        {
            logger.LogError($"Couldn't parse json from url {url}");
            return null;
        }

        if (artistFullInfo.resultCount == 0)
        {
            logger.LogError($"There's no artist with this id - {artistId}");
            return null;
        }

        var latestReleases = artistFullInfo.results
            .Skip(1) // First is always artist metadata
            .OrderByDescending(x => x.releaseDate)
            .FirstOrDefault();

        if (latestReleases is null)
        {
            logger.LogError("Couldn't parse latest release!");
            return null;
        }

        return new(latestReleases.collectionName, latestReleases.releaseDate);
    }
}
