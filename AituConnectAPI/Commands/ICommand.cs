using Telegram.Bot.Types;

namespace AituConnectAPI.Commands
{
    public interface ICommand
    {
        public Task HandleAsync(Update update);
        public bool CanHandle(string command);
    }
}
