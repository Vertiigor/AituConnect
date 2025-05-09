using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.Registration
{
    public class MajorStepHandler : StepHandler
    {
        public MajorStepHandler(IMessageProducer producer, UserSessionService userSessionService) : base(producer, userSessionService)
        {
        }

        public override string PipelineName => "Registration";
        public override string StepName => "ChoosingMajor";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            var payload = new RegistrationContract
            {
                ChatId = session.ChatId,
                UserName = session.Username,
                University = string.Empty,
                Major = userInput,
                MessageId = messageId,
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: "ChoosingMajor",
                payload: payload,
                exchange: "aituBot.exchange",
                routingKey: "user.registration"
            );

            Console.WriteLine($"User {session.ChatId} selected major: {userInput}");

            // Update the session in Redis
            session.CurrentStep = "ChoosingUniversity"; // Update to the next step
            await _userSessionService.SetSessionAsync(session);
        }
    }
}
