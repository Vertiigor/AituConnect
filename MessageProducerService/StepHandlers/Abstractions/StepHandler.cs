using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Abstractions
{
    public abstract class StepHandler
    {
        public abstract string StepName { get; }

        protected readonly IUserService _userService;
        protected readonly BotMessageSender _botMessageSender;
        protected readonly KeyboardMarkupBuilder _keyboardMarkupBuilder;
        protected readonly ITelegramBotClient _telegramBotClient;

        public StepHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _keyboardMarkupBuilder = keyboardMarkupBuilder;
            _telegramBotClient = telegramBotClient;
        }

        public virtual async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<ListPostsContract>();
            var chatId = payload.ChatId;
            var user = await _userService.GetByChatIdAsync(chatId);

            if (user == null)
            {
                await _botMessageSender.SendTextMessageAsync(chatId, "You're not registered. Use /start command");
                return;
            }
        }
    }
}
