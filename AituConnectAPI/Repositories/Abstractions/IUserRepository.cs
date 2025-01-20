using AituConnectAPI.Models;

namespace AituConnectAPI.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
    }
}
