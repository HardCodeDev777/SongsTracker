using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;

namespace SongsTracker.Extensions.ServicesExtensions;

public static class DbServiceExtensions
{
    public static void AddTrackerDb(this WebApplicationBuilder builder, string path)
    {
        var dbPath = Path.Combine(path, "Tracker.db");

        builder.Services.AddDbContext<TrackerDbContext>(
            options => options.UseSqlite($"Data Source={dbPath}"),
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped
            );
    }
}
