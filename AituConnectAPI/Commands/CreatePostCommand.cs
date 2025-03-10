﻿using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AituConnectAPI.Commands
{
    public class CreatePostCommand : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly PipelineHandler _pipeline;
        private readonly IPostService _postService;

        public CreatePostCommand(ITelegramBotClient botClient, IUserService userService, IPipelineContextService pipelineContextService, PipelineHandler pipeline, IPostService postService)
        {
            _botClient = botClient;
            _userService = userService;
            _pipelineContextService = pipelineContextService;
            _pipeline = pipeline;
            _postService = postService;
        }

        public bool CanHandle(string command) => command.Equals("/create_post", StringComparison.OrdinalIgnoreCase);

        public async Task HandleAsync(Update update)
        {
            if (update.Message == null) return;

            var chatId = update.Message.Chat.Id.ToString();
            var username = update.Message.Chat.Username ?? "Unknown";

            var user = await _userService.GetByChatIdAsync(chatId);

            var isAdded = await _userService.DoesUserExist(user);

            if (isAdded)
            {
                var post = new Post()
                {
                    Id = Guid.NewGuid().ToString(),
                    AuthorId = user.Id,
                    Title = string.Empty,
                    Content = string.Empty,
                    CreationDate = DateTime.UtcNow,
                    Status = PostStatus.Draft,
                    University = user.University
                };
                var context = new PipelineContext()
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatId = chatId,
                    Type = PipelineType.PostCreation,
                    CurrentStep = PipelineStepType.Title,
                    Content = string.Empty,
                    IsCompleted = false
                };

                await _postService.AddAsync(post);
                await _pipelineContextService.AddAsync(context);
                await _pipeline.HandlePipelineAsync(context);
            }
            else
            {
                await _botClient.SendMessage(chatId, "You are not registered. Please, use /start command to register.");
            }
        }
    }
}
