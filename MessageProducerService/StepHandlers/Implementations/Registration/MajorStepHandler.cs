using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Models;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;

namespace MessageProducerService.StepHandlers.Implementations.Registration
{
    public class MajorStepHandler : StepHandler
    {
        public override string StepName => "ChoosingMajor";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;

        public MajorStepHandler(IUserService userService, BotMessageSender botMessageSender)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
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
        }
    }
}
