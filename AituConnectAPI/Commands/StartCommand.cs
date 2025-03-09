using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AituConnectAPI.Commands
{
    public class StartCommand : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly PipelineHandler _pipeline;

        public StartCommand(ITelegramBotClient botClient, IUserService userService, IPipelineContextService pipelineContextService, PipelineHandler pipelineHandler)
        {
            _botClient = botClient;
            _pipelineContextService = pipelineContextService;
            _userService = userService;
            _pipeline = pipelineHandler;
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
                await _userService.AddAsync(new Models.User
                {
                    ChatId = chatId,
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    Role = Roles.User,
                    JoinedDate = DateTime.UtcNow,
                    University = string.Empty,
                    Faculty = string.Empty
                });

                var context = new PipelineContext()
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatId = chatId,
                    Type = PipelineType.Registration,
                    CurrentStep = PipelineStepType.Univeristy,
                    Content = string.Empty,
                    IsCompleted = false
                };

                await _pipelineContextService.AddAsync(context);
                await _pipeline.HandlePipelineAsync(context);
            }
            else
            {
                await _botClient.SendMessage(chatId, "You are already registered!");
            }
        }
    }
}
