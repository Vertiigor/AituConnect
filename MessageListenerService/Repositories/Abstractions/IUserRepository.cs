using MessageListenerService.Models;
using MessageListenerService.Repositories.Abstractions;

namespace MessageListenerService.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
    }
}
