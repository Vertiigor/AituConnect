using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IRoleService
    {
        public Task AssignRoleAsync(string userId, Roles role);
        public Task<bool> IsInRoleAsync(string userId, Roles role);
    }
}
