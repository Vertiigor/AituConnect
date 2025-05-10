using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class StartCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;

        public StartCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/start", StringComparison.OrdinalIgnoreCase);


        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var session = await _userSessionService.GetSessionAsync(chatId);

            var exist = await _userService.DoesUserExist(chatId);

            if (!exist)
            {
                session = new UserSession
                {
                    ChatId = chatId,
                    Username = username,
                    CurrentPipeline = "Registration",
                    CurrentStep = "ChoosingMajor",
                };

                await _userSessionService.SetSessionAsync(session);

                var payload = new RegistrationContract
                {
                    ChatId = chatId,
                    University = string.Empty,
                    UserName = username,
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
