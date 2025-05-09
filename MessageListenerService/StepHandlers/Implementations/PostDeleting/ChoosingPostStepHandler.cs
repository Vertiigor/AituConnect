using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.PostDeleting
{
    public class ChoosingPostStepHandler : StepHandler
    {
        private readonly IUserService _userService;

        public ChoosingPostStepHandler(IMessageProducer producer, UserSessionService userSessionService, IUserService userService) : base(producer, userSessionService)
        {
            _userService = userService;
        }

        public override string PipelineName => "PostDeleting";
        public override string StepName => "ChoosingPost";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            var user = await _userService.GetByChatIdAsync(session.ChatId);

            var payload = new PostDeletingContract
            {
                ChatId = session.ChatId,
                PostId = userInput,
                UserId = user.Id,
                MessageId = messageId,
            };

            // Send the message to the producer
            await _producer.PublishMessageAsync(
                eventType: "ChoosingPost",
                payload: payload,
                exchange: "aituBot.exchange",
                routingKey: "post.deleting"
            );

            // Delete the session in Redis
            await _userSessionService.ClearSessionAsync(session.ChatId);
        }
    }
}
