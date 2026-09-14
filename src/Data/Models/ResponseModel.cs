namespace SongsTracker.Data.Models;

public record ResponseRootobject(int resultCount, ResponseResultObject[] results);

/// <summary>
/// First element is always artist data. Next are songs data
/// </summary>
public record ResponseResultObject(string artistName, int artistId, 
    string collectionName, DateTime releaseDate);
