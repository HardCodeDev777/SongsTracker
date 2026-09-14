using SongsTracker.Bot.Commands;

namespace SongsTracker.Extensions.ServicesExtensions;

public static class BotCommandsServicesExtensions
{
    public static void AddBotCommandServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBotCommand, AddArtistCommand>();
        builder.Services.AddScoped<IBotCommand, RemoveArtistCommand>();
        builder.Services.AddScoped<IBotCommand, ListTrackedArtistsCommand>();
    }
}
