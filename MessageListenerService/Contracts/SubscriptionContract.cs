namespace MessageListenerService.Contracts
{
    public class SubscriptionContract : IMessagePayload
    {
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string SubscriberUsername { get; set; }
        public string MessageId { get; set; }
    }
}
