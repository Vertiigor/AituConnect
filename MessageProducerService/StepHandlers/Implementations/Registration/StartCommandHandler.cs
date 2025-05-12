using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Models;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.Registration
{
    public class StartCommandHandler : StepHandler
    {
        public StartCommandHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
        }

        public override string StepName => "StartCommand";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            await base.HandleAsync(envelope);

            var payload = envelope.GetPayload<RegistrationContract>();

            Console.WriteLine($"Received message: {envelope.EventType}");
            Console.WriteLine($"ChatId: {payload.ChatId}");
            Console.WriteLine($"UserName: {payload.UserName}");

            var chatId = payload.ChatId;

            // Check if the user already exists
            var existingUser = await _userService.GetByChatIdAsync(chatId);

            if (existingUser != null)
            {
                // User already exists, send a message and return
                await _botMessageSender.SendTextMessageAsync(chatId, "You are already registered.");
                return;
            }

            // Create a new user
            var newUser = new User
            {
                ChatId = chatId,
                UserName = payload.UserName,
                Id = Guid.NewGuid().ToString(),
                University = payload.University,
                Major = payload.Major,
                JoinedDate = DateTime.UtcNow,
            };

            await _userService.AddAsync(newUser);
            // Send a welcome message to the user
            await _botMessageSender.SendTextMessageAsync(chatId, "Welcome to the registration process! Please follow the instructions.");
            await _botMessageSender.SendTextMessageAsync(chatId, "Please provide your major.");
        }
    }
}
