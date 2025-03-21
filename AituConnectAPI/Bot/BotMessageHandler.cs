using AituConnectAPI.Models;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot.Types;

namespace AituConnectAPI.Bot
{
    public class BotMessageHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IUserService _userService;
        private readonly CallbackHandler _callbackHandler;
        private readonly PipelineHandler _pipeline;
        private readonly BotMessageSender _messageSender;

        public BotMessageHandler(IServiceScopeFactory scopeFactory, IPipelineContextService pipelineContextService, IUserService userService, CallbackHandler callbackHandler, PipelineHandler pipelineHandler, BotMessageSender messageSender)
        {
            _scopeFactory = scopeFactory;
            _pipelineContextService = pipelineContextService;
            _userService = userService;
            _callbackHandler = callbackHandler;
            _pipeline = pipelineHandler;
            _messageSender = messageSender;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var context = await _pipelineContextService.GetByChatIdAsync(chatId);

            bool isCommand = update.Message.Text.StartsWith("/");

            if (context != null && isCommand)
            {
                await _messageSender.SendTextMessageAsync(chatId, "You are already in the middle of a process. Please, complete it first.");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<CommandDispatcher>();
            await dispatcher.DispatchAsync(update);
        }

        private async Task OnCallbackQueryReceived(CallbackQuery query) => await _callbackHandler.HandleCallbackAsync(query);


        public async Task HandleMessageAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var user = await _userService.GetByChatIdAsync(chatId);

            if (user == null) return;

            var context = await _pipelineContextService.GetByChatIdAsync(chatId);

            if (context == null) return;

            bool isCommand = update.Message.Text.StartsWith("/");

            // Ignore commands
            if (isCommand)
            {
                return;
            }

            if (!string.IsNullOrEmpty(update.Message.Text))
            {
                // Update the context with the user's input
                context.Content = update.Message.Text;

                // Save the updated context
                await _pipelineContextService.UpdateAsync(context);
            }

            await OnCallbackQueryReceived(update.CallbackQuery);

            await OnPipelineUpdate(context);
            await OnPipelineUpdate(context);
        }

        private async Task OnPipelineUpdate(PipelineContext context)
        {
            await _pipeline.HandlePipelineAsync(context);
        }
    }
}