using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.Registration
{
    public class UniversityStepHandler : StepHandler
    {
        public UniversityStepHandler(IMessageProducer producer, UserSessionService userSessionService) : base(producer, userSessionService)
        {
        }

        public override string StepName => "ChoosingUniversity";

        public override string PipelineName => "Registration";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            Console.WriteLine($"Handling university step for user {session.ChatId} with input: {userInput}");
            var payload = new RegistrationContract
            {
                ChatId = session.ChatId,
                UserName = session.Username,
                University = userInput,
                Major = string.Empty,
                MessageId = messageId,
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: "ChoosingUniversity",
                payload: payload,
                exchange: "aituBot.exchange",
                routingKey: "user.registration"
            );

            Console.WriteLine($"User {session.ChatId} selected university: {userInput}");

            // Delete the session in Redis
            await _userSessionService.ClearSessionAsync(session.ChatId);
        }
    }
}
