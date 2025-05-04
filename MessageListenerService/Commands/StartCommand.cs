using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class StartCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;

        public StartCommand(IMessageProducer producer, UserSessionService userSessionService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
        }

        public bool CanHandle(string command) => command.Equals("/start", StringComparison.OrdinalIgnoreCase);


        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var session = await _userSessionService.GetSessionAsync(chatId);

            var isAdded = session != null;

            if (!isAdded)
            {
                session = new UserSession
                {
                    ChatId = chatId,
                    CurrentPipeline = "Registration",
                    CurrentStep = "ChoosingUniversity",
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
                    eventType: "ChoosingUniversity",
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
