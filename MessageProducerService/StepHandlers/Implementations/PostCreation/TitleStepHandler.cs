using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.PostCreation
{
    public class TitleStepHandler : StepHandler
    {
        private readonly IPostService _postService;

        public TitleStepHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
            _postService = postService;
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
