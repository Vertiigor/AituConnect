using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(User user)
        {
            if (await DoesUserExist(user))
            {
                return;
            }

            await _userRepository.AddUserAsync(user);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> DoesUserExist(User user)
        {
            if (user == null) return false;

            return await _userRepository.GetByChatIdAsync(user.ChatId) != null;
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
