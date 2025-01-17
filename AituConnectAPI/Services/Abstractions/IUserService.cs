using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IUserService
    {
        public Task<User> GetByIdAsync(string id);
        public Task<User> GetByChatIdAsync(string chatId);
        public Task AddUserAsync(User user);
        public Task<bool> DoesUserExist(User user);
    }
}
