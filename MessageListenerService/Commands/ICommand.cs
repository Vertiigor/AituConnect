using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public interface ICommand
    {
        public Task HandleAsync(Update update);
        public bool CanHandle(string command);
    }
}
