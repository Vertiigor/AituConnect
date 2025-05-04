using MessageListenerService.Contracts;

namespace MessageListenerService.Producers.Abstractions
{
    public interface IMessageProducer
    {
        public Task PublishMessageAsync<T>(
            string eventType,
            T payload,
            string queue,
            bool durable = false,
            bool exlusive = false,
            bool autoDelete = false,
            Dictionary<string, object?> arguments = null,
            string exchange = "")
            where T : IMessagePayload;
    }
}
