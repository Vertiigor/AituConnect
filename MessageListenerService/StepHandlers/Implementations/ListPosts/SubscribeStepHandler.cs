using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.ListPosts
{
    public class SubscribeStepHandler : StepHandler
    {
        private readonly InputValidator _inputValidator;
        private readonly IUserService _userService;

        public SubscribeStepHandler(IMessageProducer producer, UserSessionService userSessionService, InputValidator inputValidator, IUserService userService) : base(producer, userSessionService)
        {
            _inputValidator = inputValidator;
            _userService = userService;
        }

        public override string StepName => "ChoosingPost";

        public override string PipelineName => "Subscription";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            if (_inputValidator.TryValidate(userInput, "UserId", out string userId))
            {
                var user = await _userService.GetByChatIdAsync(session.ChatId);

                var payload = new SubscriptionContract
                {
                    ChatId = session.ChatId,
                    UserId = userId,
                    SubscriberUsername = user.UserName,
                    MessageId = messageId,
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: "Subscribed",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "notification.subscription"
                );

                // Delete the session in Redis
                await _userSessionService.ClearSessionAsync(session.ChatId);
            }
        }
    }
}
