using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditUniversityStepHandler : StepHandler
    {
        public override string StepName => "EditingUniversity";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly ITelegramBotClient _botClient;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public EditUniversityStepHandler(IUserService userService, BotMessageSender botMessageSender,ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboard)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _botClient = telegramBotClient;
            _keyboardMarkup = keyboard;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();

            Console.WriteLine($"Received message: {envelope.EventType}");
            Console.WriteLine($"ChatId: {payload.ChatId}");
            Console.WriteLine($"UserName: {payload.UserName}");

            var chatId = payload.ChatId;

            var user = await _userService.GetByChatIdAsync(chatId);

            List<string> universities = new List<string> { "AITU", "MIT", "Harvard", "UCLA", "University of Utah" };
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            foreach (var university in universities)
            {
                var button = _keyboardMarkup.InitializeInlineKeyboardButton(university, university);
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

            await _keyboardMarkup.RemoveKeyboardAsync(_botClient, payload.ChatId, Convert.ToInt32(payload.MessageId));
            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, $"🎓Enter the name of university you're styding in:\nCurrent: {user.University}", replyMarkup: keyboard);

        }
    }
}
