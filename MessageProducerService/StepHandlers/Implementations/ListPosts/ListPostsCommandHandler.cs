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

        private readonly IPostService _postService;

        public ListPostsCommandHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
            : base(userService, botMessageSender, keyboardMarkupBuilder, telegramBotClient)
        {
            _postService = postService;
        }

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            await base.HandleAsync(envelope);

            var payload = envelope.GetPayload<ListPostsContract>();

            var chatId = payload.ChatId;
            var user = await _userService.GetByChatIdAsync(chatId);
            var university = user.University;

            var posts = await _postService.GetAllByUniversity(university);

            if (posts.Count() == 0)
            {
                await _botMessageSender.SendTextMessageAsync(chatId, "No posts available for your university.");
                return;
            }

            foreach (var post in posts)
            {
                Console.WriteLine($"Post: {post.University} {post.User.Id} {post.Title}\n{user.Id}");
                if (post.User.Id != user.Id)
                {
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                    var button = _keyboardMarkupBuilder.InitializeInlineKeyboardButton("✅ Schedule ✅", $"UserId:{post.User.Id}");
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
