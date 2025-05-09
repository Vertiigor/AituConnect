using MessageListenerService.Models;
using MessageListenerService.Repositories.Abstractions;
using MessageListenerService.Services.Abstractions;

namespace MessageListenerService.Services.Implementations
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> DoesUserExist(string chatId)
        {
            return await _userRepository.GetByChatIdAsync(chatId) != null;
        }

        public async Task<User> GetByChatIdAsync(string chatId)
        {
            var user = await _userRepository.GetByChatIdAsync(chatId);
            if (user == null)
            {
                return null;
            }

            return user;
        }
    }
}
