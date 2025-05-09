using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.PostCreation
{
    public class TitleStepHandler : StepHandler
    {
        public TitleStepHandler(IMessageProducer producer, UserSessionService userSessionService) : base(producer, userSessionService)
        {
        }

        public override string PipelineName => "PostCreation";
        public override string StepName => "TypingTitle";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            var payload = new PostCreationContract
            {
                ChatId = session.ChatId,
                MessageId = messageId,
                Title = userInput,
                Content = string.Empty,
                University = string.Empty,
                SubjectId = string.Empty
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: "TypingTitle",
                payload: payload,
                exchange: "aituBot.exchange",
                routingKey: "post.creation"
            );

            // Update the session in Redis
            session.CurrentStep = "TypingContent"; // Update to the next step
            await _userSessionService.SetSessionAsync(session);
        }
    }
}
