using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.Registration
{
    public class UniversityStepHandler : StepHandler
    {
        public override string StepName => "ChoosingUniversity";

        public UniversityStepHandler(IUserService userService, BotMessageSender botMessageSender, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();

            var chatId = payload.ChatId;

            // Check if the user already exists
            var existingUser = await _userService.GetByChatIdAsync(chatId);

            if (existingUser == null)
            {
                // User already exists, send a message and return
                await _botMessageSender.SendTextMessageAsync(chatId, "You are not registered. Type the \"start\" command.");
                return;
            }

            // Create a new user
            existingUser.University = payload.University;

            await _userService.UpdateAsync(existingUser);

            Console.WriteLine(payload.MessageId);

            await _keyboardMarkupBuilder.RemoveKeyboardAsync(_telegramBotClient, payload.ChatId, Convert.ToInt32(payload.MessageId));

            await _botMessageSender.SendTextMessageAsync(chatId, "You have successfully registered!");
        }
    }
}
