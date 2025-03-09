using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Policies.Implementations
{
    public class AccessPolicy
    {
        private readonly IRoleService _roleService;

        public AccessPolicy(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<bool> CanEditPostsAsync(string userId)
        {
            return await _roleService.IsInRoleAsync(userId, Models.Roles.Admin);
        }
    }
}
