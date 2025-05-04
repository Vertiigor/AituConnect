using StackExchange.Redis;

namespace MessageListenerService.Data.Connections.Redis
{
    public interface IRedisConnection
    {
        Task<IConnectionMultiplexer> GetConnectionAsync();
    }
}
