using Telegram.Bot.Types;

namespace AituConnectAPI.Bot
{
    public class BotMessageHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BotMessageHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            //if (update.Message == null) return;

            //var chatId = update.Message.Chat.Id.ToString();
            //var context = await _pipelineContextService.GetByChatIdAsync(chatId);

            bool isCommand = update.Message.Text.StartsWith("/");

            //if (isCommand)
            //{
            //    //await _messageSender.SendTextMessageAsync(chatId, "You are already in the middle of a process. Please, complete it first.");
            //    Console.WriteLine("You are already in the middle of a process. Please, complete it first.");
            //    return;
            //}

            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<CommandDispatcher>();

            await dispatcher.DispatchAsync(update);
        }

        public async Task HandleMessageAsync(Update update)
        {
            await HandleUpdateAsync(update);

            Console.WriteLine(update.Message?.Text);
        }
    }
}