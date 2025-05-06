using MessageProducerService.Models;

namespace MessageProducerService.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByChatIdAsync(string chatId);
    }
}
