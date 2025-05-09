using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.Services.Abstractions;
using Telegram.Bot.Types;

namespace MessageListenerService.Commands
{
    public class EditProfileCommand : ICommand
    {
        private readonly UserSessionService _userSessionService;
        private readonly IMessageProducer _producer;
        private readonly IUserService _userService;

        public EditProfileCommand(IMessageProducer producer, UserSessionService userSessionService, IUserService userService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/edit_profile", StringComparison.OrdinalIgnoreCase);

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
                    CurrentPipeline = "EditingProfile",
                    CurrentStep = "ChoosingOption",
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
                    eventType: "EditProfileCommand",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "user.editProfile"
                    );

                Console.WriteLine($"User {username} started the profile editing process.");
            }
            else
            {
                Console.WriteLine("You are not registered!");
            }
        }
    }
}
