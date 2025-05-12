using MessageListenerService.Bot;
using MessageListenerService.Services;
using Telegram.Bot.Types;

namespace AituConnectAPI.Bot
{
    public class BotMessageHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly UserSessionService _userSessionService;
        private readonly HandlerRouter _handlerRouter;

        public BotMessageHandler(IServiceScopeFactory scopeFactory, UserSessionService userSessionService, HandlerRouter handlerRouter)
        {
            _scopeFactory = scopeFactory;
            _userSessionService = userSessionService;
            _handlerRouter = handlerRouter;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            if (update.Message == null || update.Message.Text == null) return;

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

            var chatId = update.Message?.Chat.Id.ToString();
            var message = update.Message?.Text;
            var messageId = Convert.ToString(update.Message?.Id);

            if (update.Message == null  || update.Message.Text == null || update.Message.Text.StartsWith("/")) return;

            var session = await _userSessionService.GetSessionAsync(chatId);

            if (session == null) return;

            var key = (session.CurrentPipeline, session.CurrentStep);

            _handlerRouter.TryGetValue(key, out var handler);

            if (handler != null)
            {
                await handler.HandleAsync(session, message, messageId);
            }
            else
            {
                Console.WriteLine($"No handler found for pipeline: {session.CurrentPipeline}, step: {session.CurrentStep}");
            }
        }
    }
}