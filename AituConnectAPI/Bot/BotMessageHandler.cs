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

        public BotMessageHandler(IServiceScopeFactory scopeFactory, IPipelineContextService pipelineContextService, IUserService userService, CallbackHandler callbackHandler, PipelineHandler pipelineHandler)
        {
            _scopeFactory = scopeFactory;
            _pipelineContextService = pipelineContextService;
            _userService = userService;
            _callbackHandler = callbackHandler;
            _pipeline = pipelineHandler;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<CommandDispatcher>();
            await dispatcher.DispatchAsync(update);
        }

        private async Task OnCallbackQueryReceived(CallbackQuery query)
        {
            await _callbackHandler.HandleCallbackAsync(query);
        }


        public async Task HandleMessageAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var user = await _userService.GetByChatIdAsync(chatId);

            if (user == null) return;

            var context = await _pipelineContextService.GetByChatIdAsync(chatId);

            if (context == null) return;

            // Ignore commands
            if (update.Message.Text.StartsWith("/"))
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