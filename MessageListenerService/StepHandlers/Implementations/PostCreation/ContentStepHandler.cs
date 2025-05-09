using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.PostCreation
{
    public class ContentStepHandler : StepHandler
    {
        public ContentStepHandler(IMessageProducer producer, UserSessionService userSessionService) : base(producer, userSessionService)
        {
        }

        public override string PipelineName => "PostCreation";
        public override string StepName => "TypingContent";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            var payload = new PostCreationContract
            {
                ChatId = session.ChatId,
                MessageId = messageId,
                Title = string.Empty,
                Content = userInput,
                University = string.Empty,
                SubjectId = string.Empty
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: "TypingContent",
                payload: payload,
                exchange: "aituBot.exchange",
                routingKey: "post.creation"
            );

            // Update the session in Redis
            session.CurrentStep = "ChoosingSubject"; // Update to the next step
            await _userSessionService.SetSessionAsync(session);
        }
    }
}
