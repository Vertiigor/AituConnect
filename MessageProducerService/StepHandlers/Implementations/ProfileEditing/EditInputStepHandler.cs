using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditInputStepHandler : StepHandler
    {
        public override string StepName => "EditingInput";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly ITelegramBotClient _botClient;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public EditInputStepHandler(IUserService userService, BotMessageSender botMessageSender, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _botClient = telegramBotClient;
            _keyboardMarkup = keyboardMarkupBuilder;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();
            var chatId = payload.ChatId;

            var user = await _userService.GetByChatIdAsync(chatId);

            user.University = payload.University;
            user.Major = payload.Major;

            await _userService.UpdateAsync(user);

            // Send a message to the user
            await _botMessageSender.SendTextMessageAsync(chatId, "Your profile has been updated successfully!");

            await _keyboardMarkup.RemoveKeyboardAsync(_botClient, payload.ChatId, Convert.ToInt32(payload.MessageId));
        }
    }
}
