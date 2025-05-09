using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class ListPostsCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;

        public ListPostsCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/list_posts", StringComparison.OrdinalIgnoreCase);


        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var session = await _userSessionService.GetSessionAsync(chatId);

            var exist = await _userService.DoesUserExist(chatId);
            var user = await _userService.GetByChatIdAsync(chatId);

            if (exist)
            {
                session = new UserSession
                {
                    ChatId = chatId,
                    Username = username,
                    CurrentPipeline = "Subscription",
                    CurrentStep = "ChoosingPost",
                };

                await _userSessionService.SetSessionAsync(session);

                var payload = new ListPostsContract
                {
                    ChatId = chatId,
                    UserId = user.Id,
                    MessageId = update.Message.MessageId.ToString(),
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: "ListPostsCommand",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "post.list"
                    );
            }
            else
            {
                Console.WriteLine("You are not registered!");
            }
        }
    }
}
