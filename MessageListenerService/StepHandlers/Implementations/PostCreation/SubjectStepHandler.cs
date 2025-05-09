using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.PostCreation
{
    public class SubjectStepHandler : StepHandler
    {
        private readonly InputValidator _inputValidator;

        public SubjectStepHandler(IMessageProducer producer, UserSessionService userSessionService, InputValidator inputValidator) : base(producer, userSessionService)
        {
            _inputValidator = inputValidator;
        }

        public override string PipelineName => "PostCreation";
        public override string StepName => "ChoosingSubject";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            if (_inputValidator.TryValidate(userInput, "SubjectId", out string subjectId))
            {
                var payload = new PostCreationContract
                {
                    ChatId = session.ChatId,
                    MessageId = messageId,
                    Title = string.Empty,
                    Content = string.Empty,
                    University = string.Empty,
                    SubjectId = subjectId
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: "ChoosingSubject",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "post.creation"
                );

                if (subjectId == "cancel")
                {
                    await _userSessionService.ClearSessionAsync(session.ChatId);
                    return;
                }
            }
        }
    }
}
