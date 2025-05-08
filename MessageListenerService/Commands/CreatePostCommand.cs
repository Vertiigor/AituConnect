using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class CreatePostCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;

        public CreatePostCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/create_post", StringComparison.OrdinalIgnoreCase);

        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var exist = await _userService.DoesUserExist(chatId);

            if (exist)
            {
                var session = new UserSession
                {
                    ChatId = chatId,
                    Username = username,
                    CurrentPipeline = "PostCreation",
                    CurrentStep = "TypingTitle",
                };

                await _userSessionService.SetSessionAsync(session);

                var payload = new RegistrationContract
                {
                    ChatId = chatId,
                    University = string.Empty,
                    Major = string.Empty
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: "StartCommand",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "user.registration"
                    );

                Console.WriteLine($"User {username} started the registration process.");
            }
            else
            {
                Console.WriteLine("You are already registered!");
            }
        }
    }
}
