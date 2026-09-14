using Telegram.Bot;
using Telegram.Bot.Types;

namespace SongsTracker.Bot.Commands;

public interface IBotCommand
{
    string CommandName { get; }
    Task ExecuteAsync(ITelegramBotClient bot, Message message, 
        CancellationToken cancellationToken);
}
