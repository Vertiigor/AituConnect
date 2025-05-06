using MessageProducerService.Data;
using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MessageProducerService.Repositories.Implementations
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
