using MessageListenerService.Models;

namespace MessageListenerService.Services.Abstractions
{
    public interface IUserService : IService<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
        public Task<bool> DoesUserExist(string chatId);
    }
}
