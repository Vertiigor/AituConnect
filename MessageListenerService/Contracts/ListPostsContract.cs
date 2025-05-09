namespace MessageListenerService.Contracts
{
    public class ListPostsContract : IMessagePayload
    {
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}
