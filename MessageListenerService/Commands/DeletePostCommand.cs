using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services.Abstractions;
using MessageListenerService.Services;
using Telegram.Bot.Types;
using MessageListenerService.Models;
using MessageListenerService.Contracts;

namespace MessageListenerService.Commands
{
    public class DeletePostCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public DeletePostCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService, IPostService postService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
            _postService = postService;
        }

        public bool CanHandle(string command) => command.Equals("/delete_post", StringComparison.OrdinalIgnoreCase);

        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var exist = await _userService.DoesUserExist(chatId);
            var user = await _userService.GetByChatIdAsync(chatId);
            var posts = await _postService.GetAllPostsByUserId(user.Id);

            if (exist && posts != null)
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
