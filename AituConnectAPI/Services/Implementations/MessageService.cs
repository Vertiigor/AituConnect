using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class MessageService : Service<Message>, IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository repository) : base(repository)
        {
            _messageRepository = repository;
        }
    }
}
