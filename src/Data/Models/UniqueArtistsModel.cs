namespace SongsTracker.Data.Models;

public record UniqueArtistsModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset LastReleaseDate { get; set; }
    public List<UserModel> Followers { get; set; } = new();
}
