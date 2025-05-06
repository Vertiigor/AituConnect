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
                        update.Message = new Telegram.Bot.Types.Message
                        {
                            Chat = update.CallbackQuery.Message.Chat,
                            From = update.CallbackQuery.From,
                            Text = update.CallbackQuery.Data,
                            Id = update.CallbackQuery.Message.MessageId,
                        };

                        update.Message.Text = update.CallbackQuery.Data;
                        Console.WriteLine(update.CallbackQuery.Message.Text);
                    }

                    await handler.HandleMessageAsync(update);
                },
                async (bot, exception, token) => Console.WriteLine(exception.Message),
                new Telegram.Bot.Polling.ReceiverOptions()
            );
        }
    }
}
