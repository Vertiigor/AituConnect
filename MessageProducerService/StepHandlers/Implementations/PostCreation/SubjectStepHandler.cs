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
        private readonly BotMessageSender _botMessageSender;
        private readonly IUserService _userService;
        private readonly ISubjectService _subjectService;
        private readonly KeyboardMarkupBuilder _keyboardMarkupBuilder;
        private readonly ITelegramBotClient _botClient;

        public SubjectStepHandler(IPostService postService, BotMessageSender botMessageSender, IUserService userService, ISubjectService subjectService, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient)
        {
            _postService = postService;
            _botMessageSender = botMessageSender;
            _userService = userService;
            _subjectService = subjectService;
            _keyboardMarkupBuilder = keyboardMarkupBuilder;
            _botClient = telegramBotClient;
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

                await _keyboardMarkupBuilder.RemoveKeyboardAsync(_botClient, chatId, Convert.ToInt32(payload.MessageId));

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
