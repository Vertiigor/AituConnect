using MessageListenerService.Models;

namespace MessageListenerService.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
    }
}
