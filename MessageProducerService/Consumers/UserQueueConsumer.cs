using MessageProducerService.Data.Connections.RabbitMq;

namespace MessageProducerService.Consumers
{
    public class UserQueueConsumer : BackgroundService
    {
        protected readonly IRabbitMqConnection _rabbitMqConnection;

        public UserQueueConsumer(IRabbitMqConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //throw new NotImplementedException();
        }
    }
}
