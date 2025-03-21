using AituConnectAPI.Models;

namespace AituConnectAPI.Repositories.Abstractions
{
    public interface IMessageRepository : IRepository<Message>
    {
        public Task<IEnumerable<Message>> GetAllByChatIdAsync(string chatId);
    }
}
