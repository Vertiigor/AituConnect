using RabbitMQ.Client;

namespace MessageListenerService.Data.Connections.RabbitMq
{
    public interface IRabbitMqConnection
    {
        public Task<IConnection> GetConnectionAsync();
    }
}
