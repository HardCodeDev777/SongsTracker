using Serilog;
using Serilog.Events;
using SongsTracker.Bot;
using SongsTracker.Extensions.MiddlewareExtensions;
using SongsTracker.Extensions.ServicesExtensions;
using SongsTracker.MusicRequests;
using Telegram.Bot;

var dataFolder = Path.Combine(AppContext.BaseDirectory, "db");
if (!Directory.Exists(dataFolder))
    Directory.CreateDirectory(dataFolder);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(dataFolder, "logs/bot-log-.txt"),
        rollingInterval: RollingInterval.Minute)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    var envData = builder.AddEnvService();

    builder.Services.AddHttpClient<IMusicService, ItunesService>();

    var botClient = new TelegramBotClient(envData.TelegramApiKey);
    builder.Services.AddSingleton<ITelegramBotClient>(botClient);
    builder.AddBotCommandServices();

    builder.Services.AddHostedService<TelegramBotWorker>();
    builder.Services.AddHostedService<SongsTrackWorker>();

    builder.AddTrackerDb(dataFolder);

    var app = builder.Build();

    await app.MigrateDb();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal app error damn!");
}
finally
{
    Log.CloseAndFlush();
}