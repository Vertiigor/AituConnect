using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using AituConnectAPI.Models;

namespace AituConnectAPI.Commands
{
    public class StartCommand : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;

        public StartCommand(ITelegramBotClient botClient, IUserService userService)
        {
            _botClient = botClient;
            _userService = userService;
        }

        public bool CanHandle(string command) => command.Equals("/start", StringComparison.OrdinalIgnoreCase);

        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var user = await _userService.GetByChatIdAsync(chatId);

            var isAdded = await _userService.DoesUserExist(user);

            if (isAdded == false)
            {
                await _userService.AddUserAsync(new Models.User
                {
                    ChatId = chatId,
                    UserName = username,
                    NormalizedUserName = username.ToUpper()
                });
                await _botClient.SendMessage(chatId, "Welcome! You have been registered.");
            }
            else
            {
                await _botClient.SendMessage(chatId, "You are already registered!");
            }
        }
    }
}
