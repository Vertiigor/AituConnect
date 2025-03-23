using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.PostCreation
{
    public class TitleStep : PipelineStep
    {
        private readonly IPostService _postService;

        public TitleStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IPostService postService, IUserService userService) : base(messageSender, pipelineContextService, userService)
        {
            _postService = postService;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, "Enter the title of your new post: ");
            }
            else
            {
                if (context.Content.Length > 100)
                {
                    await _messageSender.SendTextMessageAsync(context.ChatId, "Title is too long. It has to be less than 100 characters. Please, enter a shorter title: ");
                    return;
                }

                var owner = await _userService.GetByChatIdAsync(context.ChatId);
                var post = await _postService.GetLastDraftByAuthorIdAsync(owner.Id);

                if (post == null)
                    return;

                post.Title = context.Content;

                await _postService.UpdateAsync(post);

                context.CurrentStep = PipelineStepType.WritingContent; // Move to the next step
                context.Content = string.Empty;
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.WritingTitle;
        }
    }
}
