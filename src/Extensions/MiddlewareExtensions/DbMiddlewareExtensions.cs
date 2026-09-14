using Microsoft.EntityFrameworkCore;
using SongsTracker.Data.Database;

namespace SongsTracker.Extensions.MiddlewareExtensions;

public static class DbMiddlewareExtensions
{
    public static async Task MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<TrackerDbContext>();
        dbContext.Database.Migrate();
    }
}
