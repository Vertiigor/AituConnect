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

        public EditInputStepHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient botClient)
            : base(userService, botMessageSender, keyboardMarkupBuilder, botClient)
        {
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

            await _keyboardMarkupBuilder.RemoveKeyboardAsync(_telegramBotClient, payload.ChatId, Convert.ToInt32(payload.MessageId));
        }
    }
}
