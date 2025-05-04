using MessageListenerService.Contracts;
using MessageListenerService.Data.Connections.RabbitMq;
using MessageListenerService.Producers.Abstractions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageListenerService.Producers.Implementations
{
    public class Producer : IMessageProducer
    {
        private readonly IRabbitMqConnection _connection;

        public Producer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishMessageAsync<T>(
            string eventType,
            T payload,
            string queue,
            bool durable = false,
            bool exlusive = false,
            bool autoDelete = false,
            Dictionary<string, object?> arguments = null,
            string exchange = "")
        {
            var connection = await _connection.GetConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queue,
                                     durable: durable,
                                     exclusive: exlusive,
                                     autoDelete: autoDelete,
                                     arguments: arguments);

            var envelope = new MessageEnvelope
            {
                EventType = eventType,
                Payload = payload
            };

            var json = JsonSerializer.Serialize(envelope);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: exchange,
                routingKey: queue,
                body: body);
        }
    }
}
