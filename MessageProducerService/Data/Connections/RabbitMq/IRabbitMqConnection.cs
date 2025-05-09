using RabbitMQ.Client;

namespace MessageProducerService.Data.Connections.RabbitMq
{
    public interface IRabbitMqConnection
    {
        public Task<IConnection> GetConnectionAsync();
    }
}
