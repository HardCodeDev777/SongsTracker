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
        var url =
            $"https://itunes.apple.com/lookup?id={artistId}&entity=album&limit=5&sort=recent";
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

        return new(artistFullInfo.results[1].collectionName, artistFullInfo.results[1].releaseDate);
    }
}
