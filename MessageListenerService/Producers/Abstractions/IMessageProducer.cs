using MessageListenerService.Contracts;

namespace MessageListenerService.Producers.Abstractions
{
    public interface IMessageProducer
    {
        public Task PublishMessageAsync<T>(
            string eventType,
            T payload,
            string exchange = "",
            string routingKey = "",
            bool durable = true,
            Dictionary<string, object?> arguments = null)
            where T : IMessagePayload;
    }
}
