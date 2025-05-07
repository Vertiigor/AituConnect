using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditProfileCommandHandler : StepHandler
    {
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public EditProfileCommandHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkup)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _keyboardMarkup = keyboardMarkup;
        }

        public override string StepName => "EditProfileCommand";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();

            Console.WriteLine($"Received message: {envelope.EventType}");
            Console.WriteLine($"ChatId: {payload.ChatId}");
            Console.WriteLine($"UserName: {payload.UserName}");

            var chatId = payload.ChatId;

            List<string> options = new List<string> { "University", "Major" };
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            foreach (var option in options)
            {
                var button = _keyboardMarkup.InitializeInlineKeyboardButton(option, option);
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, "Please choose an option to edit your profile:", replyMarkup: keyboard);

        }
    }
}
