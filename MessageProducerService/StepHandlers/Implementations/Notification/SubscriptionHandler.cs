using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.Notification
{
    public class SubscriptionHandler : StepHandler
    {
        public override string StepName => "Subscribed";

        public SubscriptionHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
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
