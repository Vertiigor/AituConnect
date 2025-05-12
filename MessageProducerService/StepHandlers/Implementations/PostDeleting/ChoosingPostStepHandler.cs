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

        private readonly IPostService _postService;

        public ChoosingPostStepHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
            _postService = postService;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostDeletingContract>();

            var postId = payload.PostId;

            await _postService.DeleteAsync(postId);

            await _keyboardMarkupBuilder.RemoveKeyboardAsync(_telegramBotClient, payload.ChatId, Convert.ToInt32(payload.MessageId));

            await _botMessageSender.SendTextMessageAsync(payload.ChatId, "Post deleted successfully!");
        }
    }
}
