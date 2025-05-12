using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.Registration
{
    public class MajorStepHandler : StepHandler
    {
        public override string StepName => "ChoosingMajor";

        public MajorStepHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<RegistrationContract>();

            Console.WriteLine($"Received message: {envelope.EventType}");
            Console.WriteLine($"ChatId: {payload.ChatId}");
            Console.WriteLine($"UserName: {payload.UserName}");

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
            existingUser.Major = payload.Major;

            await _userService.UpdateAsync(existingUser);

            List<string> universities = new List<string> { "AITU", "MIT", "Harvard", "UCLA", "University of Utah" };
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            foreach (var university in universities)
            {
                var button = _keyboardMarkupBuilder.InitializeInlineKeyboardButton(university, $"University:{university}");
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkupBuilder.InitializeInlineKeyboardMarkup(buttons);

            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, "🎓Enter the name of university you're styding in: ", replyMarkup: keyboard);

        }
    }
}
