using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Keyboards;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessageProducerService.StepHandlers.Implementations.PostDeleting
{
    public class DeletePostCommandHandler : StepHandler
    {
        private readonly IUserService _userService;
        private readonly BotMessageSender _botMessageSender;
        private readonly IPostService _postService;
        private readonly ITelegramBotClient _botClient;
        private readonly KeyboardMarkupBuilder _keyboardMarkupBuilder;

        public DeletePostCommandHandler(IUserService userService, BotMessageSender botMessageSender, IPostService postService, ITelegramBotClient telegramBotClient, KeyboardMarkupBuilder keyboardMarkupBuilder)
        {
            _userService = userService;
            _botMessageSender = botMessageSender;
            _postService = postService;
            _botClient = telegramBotClient;
            _keyboardMarkupBuilder = keyboardMarkupBuilder;
        }

        public override string StepName => "DeletePostCommand";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostDeletingContract>();

            var chatId = payload.ChatId;
            var user = await _userService.GetByChatIdAsync(chatId);
            var posts = await _postService.GetAllPostsByUserId(user.Id);

            posts.ToList();

            if (posts.Count() == 0)
            {
                await _botMessageSender.SendTextMessageAsync(chatId, "You have no posts to delete.");
                return;
            }

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            foreach (var post in posts)
            {
                var button = _keyboardMarkupBuilder.InitializeInlineKeyboardButton(post.Title, $"PostId:{post.Id}");
                buttons.Add(button);
            }

            var keyboard = _keyboardMarkupBuilder.InitializeInlineKeyboardMarkup(buttons);

            // Ask user for the title
            await _botMessageSender.SendTextMessageAsync(chatId, "Please choose the post you want to delete.", replyMarkup: keyboard);
        }
    }
}
