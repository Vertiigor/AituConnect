using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;

namespace AituConnectAPI.Repositories.Implementations
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
