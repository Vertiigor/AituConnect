namespace MessageProducerService.Contracts
{
    public class RegistrationContract : IMessagePayload
    {
        public string ChatId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string MessageId { get; set; }
    }
}
