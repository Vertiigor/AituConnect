using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.PostCreation
{
    public class ContentStep : PipelineStep
    {
        private readonly IPostService _postService;

        public ContentStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService, IPostService postService) : base(messageSender, pipelineContextService, userService)
        {
            _postService = postService;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, "Enter the content of your new post: ");
            }
            else
            {
                var owner = await _userService.GetByChatIdAsync(context.ChatId);
                var post = await _postService.GetByAuthorIdAsync(owner.Id);
                post.Content = context.Content;

                await _postService.UpdateAsync(post);

                context.CurrentStep = "CONTENT"; // Move to the next step
                context.Content = string.Empty;
                await _pipelineContextService.DeleteAsync(context.Id);
            }
        }
        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "CONTENT";
        }
    }
}
