using MessageProducerService.Bot;
using MessageProducerService.Contracts;
using MessageProducerService.Models;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.StepHandlers.Abstractions;

namespace MessageProducerService.StepHandlers.Implementations.PostCreation
{
    public class CreatePostCommandHandler : StepHandler
    {
        private readonly IPostService _postService;
        private readonly BotMessageSender _botMessageSender;
        private readonly IUserService _userService;

        public CreatePostCommandHandler(IPostService postService, BotMessageSender botMessageSender, IUserService userService)
        {
            _postService = postService;
            _botMessageSender = botMessageSender;
            _userService = userService;
        }

        public override string StepName => "CreatePostCommand";

        public override async Task HandleAsync(MessageEnvelope envelope)
        {
            var payload = envelope.GetPayload<PostCreationContract>();

            var chatId = payload.ChatId;
            var user = await _userService.GetByChatIdAsync(chatId);

            // Create a new post
            var newPost = new Post
            {
                Id = Guid.NewGuid().ToString(),
                Title = string.Empty,
                Content = string.Empty,
                University = payload.University,
                Subjects = new List<Subject>(),
                CreatedAt = DateTime.UtcNow,
                Status = Status.Draft,
                UserId = user.Id,
            };

            await _postService.AddAsync(newPost);
            // Send a welcome message to the user
            await _botMessageSender.SendTextMessageAsync(chatId, "Welcome to the post creation process! Please follow the instructions.");
            await _botMessageSender.SendTextMessageAsync(chatId, "Please provide the title.");
        }
    }
}
