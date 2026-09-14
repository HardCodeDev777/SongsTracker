namespace SongsTracker.Data.Models;

public record UserModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<UniqueArtistsModel> TrackedArtists { get; set; } = new();
}