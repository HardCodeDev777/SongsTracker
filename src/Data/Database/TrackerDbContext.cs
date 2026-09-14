using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Models;

namespace SongsTracker.Data.Database;

public class TrackerDbContext(DbContextOptions<TrackerDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();

    public DbSet<UniqueArtistsModel> UniqueArtists => Set<UniqueArtistsModel>();
}
