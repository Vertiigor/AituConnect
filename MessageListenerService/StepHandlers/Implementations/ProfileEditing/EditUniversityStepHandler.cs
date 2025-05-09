using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.ProfileEditing
{
    public class EditUniversityStepHandler : StepHandler
    {
        public override string StepName => "EditingUniversity";
        public override string PipelineName => "EditingProfile";

        private readonly IUserService _userService;

        public EditUniversityStepHandler(IMessageProducer producer, UserSessionService userSessionService, IUserService userService) : base(producer, userSessionService)
        {
            _userService = userService;
        }

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            var user = await _userService.GetByChatIdAsync(session.ChatId);

            var payload = new RegistrationContract
            {
                ChatId = session.ChatId,
                UserName = session.Username,
                University = userInput,
                Major = user.Major,
                MessageId = messageId,
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: $"EditingInput",
                payload: payload,
                exchange: "aituBot.exchange",
            routingKey: "user.editProfile"
            );

            Console.WriteLine($"User {session.ChatId} selected university: {userInput}");

            // Update the session in Redis
            await _userSessionService.ClearSessionAsync(session.ChatId);
        }
    }
}
