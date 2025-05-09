using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.PostDeleting
{
    public class ChoosingPostStepHandler : StepHandler
    {
        public override string StepName => "ChoosingPost";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly IPostService _postService;

        public ChoosingPostStepHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _postService = postService;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostDeletingContract>();

            var postId = payload.PostId;

            await _postService.DeleteAsync(postId);

            await _botMessageSender.SendTextMessageAsync(payload.ChatId, "Post deleted successfully!");
        }
    }
}
