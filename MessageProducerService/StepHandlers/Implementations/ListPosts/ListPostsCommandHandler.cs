using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.ListPosts
{
    public class ListPostsCommandHandler : StepHandler
    {
        public override string StepName => "ListPostsCommand";
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly KeyboardMarkupBuilder _keyboardMarkupBuilder;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IPostService _postService;

        public ListPostsCommandHandler(IUserService userService, BotMessageSender botMessageSender, KeyboardMarkupBuilder keyboardMarkupBuilder, ITelegramBotClient telegramBotClient, IPostService postService)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _keyboardMarkupBuilder = keyboardMarkupBuilder;
            _telegramBotClient = telegramBotClient;
            _postService = postService;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<ListPostsContract>();

            var chatId = payload.ChatId;
            var user = await _userService.GetByChatIdAsync(chatId);
            var university = user.University;

            var posts = await _postService.GetAllByUniversity(university);

            foreach (var post in posts)
            {
                if (post.UserId != user.Id)
                {
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                    var button = _keyboardMarkupBuilder.InitializeInlineKeyboardButton("✅ Schedule ✅", post.UserId);
                    buttons.Add(button);

                    var message = $"Title: {post.Title}\n\n" +
                                  $"Username: {post.User.UserName}\n\n" +
                                  $"Subjects: {string.Join(" ", post.Subjects.Select(s => $"#{s.Name.Replace(' ', '_')}"))}\n\n" +
                                  $"{post.Content}\n\n" +
                                  $"Date: {post.CreatedAt.ToString("MM-dd-yyyy")}\n";

                    var keyboard = _keyboardMarkupBuilder.InitializeInlineKeyboardMarkup(buttons);

                    await _botMessageSender.SendTextMessageAsync(chatId, message, replyMarkup: keyboard);

                }
            }
        }
    }
}
