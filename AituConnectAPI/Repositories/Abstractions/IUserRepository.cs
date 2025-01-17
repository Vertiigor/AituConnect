using AituConnectAPI.Models;

namespace AituConnectAPI.Repositories.Abstractions
{
    public interface IUserRepository
    {
        public Task<User> GetByIdAsync(string id);
        public Task<User> GetByChatIdAsync(string chatId);
        public Task AddUserAsync(User user);

    }
}
