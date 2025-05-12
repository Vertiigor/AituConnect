using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditProfileCommandHandler : StepHandler
    {
        public EditProfileCommandHandler(IUserService userService, BotMessageSender botMessageSender, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
        }

        public override string StepName => "EditProfileCommand";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            await base.HandleAsync(envelope);

            var payload = envelope.GetPayload<RegistrationContract>();

            Console.WriteLine($"Received message: {envelope.EventType}");
            Console.WriteLine($"ChatId: {payload.ChatId}");
            Console.WriteLine($"UserName: {payload.UserName}");

            var chatId = payload.ChatId;

            List<string> options = new List<string> { "University", "Major" };
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            foreach (var option in options)
            {
                var button = _keyboardMarkupBuilder.InitializeInlineKeyboardButton(option, $"Option:{option}");
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkupBuilder.InitializeInlineKeyboardMarkup(buttons);

            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, "Please choose an option to edit your profile:", replyMarkup: keyboard);

        }
    }
}
