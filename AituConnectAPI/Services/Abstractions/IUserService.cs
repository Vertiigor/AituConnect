using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IUserService : IService<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
        public Task<bool> DoesUserExist(User user);
    }
}
