namespace MessageListenerService.Contracts
{
    public class PostDeletingContract : IMessagePayload
    {
        public string ChatId { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}
