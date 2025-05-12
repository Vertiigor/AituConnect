using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class DeletePostCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;

        public DeletePostCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/delete_post", StringComparison.OrdinalIgnoreCase);

        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var exist = await _userService.DoesUserExist(chatId);
            var user = await _userService.GetByChatIdAsync(chatId);

            if (exist)
            {
                var session = new UserSession
                {
                    ChatId = chatId,
                    Username = username,
                    CurrentPipeline = "PostDeleting",
                    CurrentStep = "ChoosingPost",
                };

                await _userSessionService.SetSessionAsync(session);

                var payload = new PostDeletingContract
                {
                    ChatId = chatId,
                    PostId = string.Empty,
                    UserId = user.Id,
                    MessageId = update.Message.MessageId.ToString()
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: "DeletePostCommand",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "post.deleting"
                    );

                Console.WriteLine($"User {username} started creating a post.");
            }
            else
            {
                Console.WriteLine("You are not registered!");
            }
        }
    }
}
