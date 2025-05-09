using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditMajorStepHandler : StepHandler
    {
        public override string StepName => "EditingMajor";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly KeyboardMarkupBuilder _keyboardMarkupBuilder;

        public EditMajorStepHandler(IUserService userService, BotMessageSender botMessageSender, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _telegramBotClient = telegramBotClient;
            _keyboardMarkupBuilder = keyboardMarkupBuilder;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();
            var chatId = payload.ChatId;

            var user = await _userService.GetByChatIdAsync(chatId);
            await _keyboardMarkupBuilder.RemoveKeyboardAsync(_telegramBotClient, payload.ChatId, Convert.ToInt32(payload.MessageId));
            await _botMessageSender.SendTextMessageAsync(chatId, $"Please provide your new major.\nCurrent: {user.Major}");
        }
    }
}
