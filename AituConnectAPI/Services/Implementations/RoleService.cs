using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;

        public RoleService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AssignRoleAsync(string userId, Roles role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.Role = role;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<bool> IsInRoleAsync(string userId, Roles role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null && user.Role == role;
        }
    }
}
