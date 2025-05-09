using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;

namespace MessageProducerService.StepHandlers.Implementations.PostCreation
{
    public class TitleStepHandler : StepHandler
    {
        private readonly IPostService _postService;
        private readonly BotMessageSender _botMessageSender;
        private readonly IUserService _userService;

        public TitleStepHandler(IPostService postService, BotMessageSender botMessageSender, IUserService userService)
        {
            _postService = postService;
            _botMessageSender = botMessageSender;
            _userService = userService;
        }

        public override string StepName => "TypingTitle";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostCreationContract>();

            var chatId = payload.ChatId;
            var title = payload.Title;

            var user = await _userService.GetByChatIdAsync(chatId);
            var existingPost = await _postService.GetLastDraftedPost(user.Id);

            existingPost.Title = title;

            await _postService.UpdateAsync(existingPost);

            await _botMessageSender.SendTextMessageAsync(chatId, "Please type the content of your post.");
        }
    }
}
