namespace MessageProducerService.Contracts
{
    public interface IMessagePayload
    {
        public string ChatId { get; set; }
        public string MessageId { get; set; }
    }
}
