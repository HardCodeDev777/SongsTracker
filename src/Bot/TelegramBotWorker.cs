using SongsTracker.Bot.Commands;
using SongsTracker.Data;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SongsTracker.Bot;

public class TelegramBotWorker(EnvData envData, IServiceScopeFactory scopeFactory, 
    ITelegramBotClient bot, ILogger<TelegramBotWorker> logger) : BackgroundService
{
    private CancellationToken _stoppingToken = CancellationToken.None;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        if (bot is TelegramBotClient botClient)
        {
            botClient.OnMessage += OnMessage;
            botClient.OnError += OnError;
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Bot error");
    }

    private async Task OnMessage(Message message, UpdateType type)
    {
        if (message.From is null || envData.AllowedTelegramId != message.From.Id) return;
        if (message.Text is null) return;

        logger.LogInformation($"Received '{message.Text}'");

        var parts = message.Text.Split(" ", 2);
        var commandName = parts[0];

        using var scope = scopeFactory.CreateScope();
        var commands = scope.ServiceProvider.GetServices<IBotCommand>();

        var command = commands.FirstOrDefault(c => c.CommandName == commandName);

        if (command is null) return;
        await command.ExecuteAsync(bot, message, _stoppingToken);   
    }
}
