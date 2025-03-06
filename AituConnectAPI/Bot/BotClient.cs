using Telegram.Bot;

namespace AituConnectAPI.Bot
{
    public class BotClient
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public BotClient(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory)
        {
            _botClient = botClient;
            _scopeFactory = scopeFactory;
        }

        public void StartReceiving()
        {
            _botClient.StartReceiving(
                async (bot, update, token) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<BotMessageHandler>();
                    if (update.CallbackQuery != null)
                    {
                        update.Message = update.CallbackQuery.Message;
                    }
                    await handler.HandleUpdateAsync(update);
                    await handler.HandleMessageAsync(update);
                },
                async (bot, exception, token) => Console.WriteLine(exception.Message),
                new Telegram.Bot.Polling.ReceiverOptions()
            );
        }
    }
}
