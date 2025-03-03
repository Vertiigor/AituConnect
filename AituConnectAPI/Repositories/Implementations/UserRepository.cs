using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AituConnectAPI.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context) { }

        public async Task<User> GetByChatIdAsync(string chatId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
        }
    }
}
