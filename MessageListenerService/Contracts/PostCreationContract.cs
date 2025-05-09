namespace MessageListenerService.Contracts
{
    public class PostCreationContract : IMessagePayload
    {
        public string ChatId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string University { get; set; }
        public string SubjectId { get; set; }
        public string MessageId { get; set; }
    }
}
