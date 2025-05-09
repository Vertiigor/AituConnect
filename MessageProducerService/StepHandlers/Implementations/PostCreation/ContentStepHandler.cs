using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.PostCreation
{
    public class ContentStepHandler : StepHandler
    {
        private readonly IPostService _postService;
        private readonly BotMessageSender _botMessageSender;
        private readonly IUserService _userService;
        private readonly ISubjectService _subjectService;
        private readonly KeyboardMarkupBuilder _keyboardMarkup;

        public ContentStepHandler(IPostService postService, BotMessageSender botMessageSender, IUserService userService, ISubjectService subjectService, KeyboardMarkupBuilder keyboardMarkupBuilder)
        {
            _postService = postService;
            _botMessageSender = botMessageSender;
            _userService = userService;
            _subjectService = subjectService;
            _keyboardMarkup = keyboardMarkupBuilder;
        }

        public override string StepName => "TypingContent";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostCreationContract>();

            var chatId = payload.ChatId;
            var content = payload.Content;

            var user = await _userService.GetByChatIdAsync(chatId);
            var existingPost = await _postService.GetLastDraftedPost(user.Id);

            existingPost.Content = content;

            await _postService.UpdateAsync(existingPost);

            var subjects = await _subjectService.GetAllAsync();
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            var cancelButton = _keyboardMarkup.InitializeInlineKeyboardButton("❌Finish❌", "SubjectId:cancel");
            buttons.Add(cancelButton);

            // Create inline keyboard buttons for each subject
            foreach (var subject in subjects)
            {
                var button = _keyboardMarkup.InitializeInlineKeyboardButton(subject.Name, $"SubjectId:{subject.Id}");
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkup.InitializeInlineKeyboardMarkup(buttons);

            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, "Please select a subject for your post:", keyboard);
        }
    }
}
