using SongsTracker.Data.Models;

namespace SongsTracker.MusicRequests;

public interface IMusicService
{
    Task<ArtistIdAndName?> GetArtistIdAndFullnameByName(string artistName,
        CancellationToken cancellationToken);

    Task<SongShortModel?> GetLatestSongShortData(long artistId,
        CancellationToken cancellationToken);
}
