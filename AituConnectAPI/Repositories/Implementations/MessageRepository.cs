using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AituConnectAPI.Repositories.Implementations
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetAllByChatIdAsync(string chatId)
        {
            return await _context.Messages.Where(m => m.ChatId == chatId).ToListAsync();
        }
    }
}
