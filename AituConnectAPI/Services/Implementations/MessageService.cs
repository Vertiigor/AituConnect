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

        public async Task<IEnumerable<Message>> GetAllByChatIdAsync(string chatId)
        {
            return await _messageRepository.GetAllByChatIdAsync(chatId);
        }

        public async Task<Message> GetLastByChatIdAsync(string chatId)
        {
            var messages = await GetAllByChatIdAsync(chatId);
            var sortedMessages = messages.OrderByDescending(m => m.SentTime).ToList();
            var lastMessage = sortedMessages.First();

            return lastMessage;
        }
    }
}
