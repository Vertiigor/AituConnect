using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;

namespace MessageProducerService.StepHandlers.Implementations.Notification
{
    public class SubscriptionHandler : StepHandler
    {
        public override string StepName => "Subscribed";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public SubscriptionHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboard)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _keyboardMarkup = keyboard;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<SubscriptionContract>();

            var user = await _userService.GetByIdAsync(payload.UserId);

            var toChatId = user.ChatId;

            var message = $"{payload.SubscriberUsername} subscribed on you";

            await _botMessageSender.SendTextMessageAsync(toChatId, message);
        }
    }
}
