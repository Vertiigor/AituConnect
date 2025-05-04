using MessageListenerService.Commands;
using Telegram.Bot.Types;

namespace AituConnectAPI.Bot
{
    public class CommandDispatcher
    {
        private readonly IEnumerable<ICommand> _commandHandlers;

        public CommandDispatcher(IEnumerable<ICommand> commandHandlers)
        {
            _commandHandlers = commandHandlers;
        }

        public async Task DispatchAsync(Update update)
        {
            if (update.Message?.Text == null) return;

            var command = update.Message.Text.Split(' ')[0];

            var handler = _commandHandlers.FirstOrDefault(h => h.CanHandle(command));
            if (handler != null)
            {
                await handler.HandleAsync(update);
            }
        }
    }
}