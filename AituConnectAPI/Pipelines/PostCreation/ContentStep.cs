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
                if (context.Content.Length > 1000)
                {
                    await _messageSender.SendTextMessageAsync(context.ChatId, "Content is too long. It has to be less than 1000 characters. Please, enter a shorter content: ");
                    return;
                }

                var owner = await _userService.GetByChatIdAsync(context.ChatId);
                var posts = await _postService.GetAllByAuthorIdAsync(owner.Id);

                List<Post> sortedPosts = posts.Where(p => p.Status == PostStatus.Draft).OrderByDescending(p => p.CreationDate).ToList();

                if (sortedPosts.Count == 0)
                    return;

                var post = sortedPosts.First();

                post.Content = context.Content;
                post.Status = PostStatus.Published;

                await _postService.UpdateAsync(post);

                context.CurrentStep = PipelineStepType.WritingContent; // Move to the next step
                await _pipelineContextService.DeleteAsync(context.Id);
            }
        }
        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.WritingContent;
        }
    }
}
