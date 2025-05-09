using MessageListenerService.Data.Connections.Redis;
using MessageListenerService.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace MessageListenerService.Services
{
    public class UserSessionService
    {
        private readonly IRedisConnection _connection;

        public UserSessionService(IRedisConnection connection)
        {
            _connection = connection;
        }

        private string GetKey(string chatId) => $"session:{chatId}";

        public async Task<UserSession?> GetSessionAsync(string chatId)
        {
            var db = await GetDatabase();

            var json = await db.StringGetAsync(GetKey(chatId));

            return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<UserSession>(json!);
        }

        private async Task<IDatabase> GetDatabase()
        {
            var connection = await _connection.GetConnectionAsync();
            var db = connection.GetDatabase();
            return db;
        }

        public async Task SetSessionAsync(UserSession session)
        {
            var db = await GetDatabase();

            var json = JsonSerializer.Serialize(session);

            await db.StringSetAsync(GetKey(session.ChatId), json, TimeSpan.FromHours(1));
        }

        public async Task ClearSessionAsync(string chatId)
        {
            var db = await GetDatabase();

            await db.KeyDeleteAsync(GetKey(chatId));
        }
    }

}
