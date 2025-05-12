using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Models;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;

namespace MessageProducerService.StepHandlers.Implementations.PostCreation
{
    public class SubjectStepHandler : StepHandler
    {
        private readonly IPostService _postService;
        private readonly ISubjectService _subjectService;

        public SubjectStepHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder, ISubjectService subjectService)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
            _postService = postService;
            _subjectService = subjectService;
        }

        public override string StepName => "ChoosingSubject";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostCreationContract>();

            var chatId = payload.ChatId;
            var subjectId = payload.SubjectId;

            var user = await _userService.GetByChatIdAsync(chatId);
            var post = await _postService.GetLastDraftedPost(user.Id);

            if (subjectId == "cancel")
            {
                post.Status = Status.Published;
                await _postService.UpdateAsync(post);

                await _keyboardMarkupBuilder.RemoveKeyboardAsync(_telegramBotClient, chatId, Convert.ToInt32(payload.MessageId));

                await _botMessageSender.SendTextMessageAsync(chatId, "Your post has been created!");
                return;
            }

            var subject = await _subjectService.GetByIdAsync(subjectId);
            post.Subjects.Add(subject);

            await _postService.UpdateAsync(post);
            await _botMessageSender.SendTextMessageAsync(chatId, "Subject added to the post.");
        }
    }
}
