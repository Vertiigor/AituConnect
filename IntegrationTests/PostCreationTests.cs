using AituConnectAPI.Bot;
using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Services.Abstractions;
using IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace IntegrationTests
{
    public class PostCreationTests : IClassFixture<TestFixture>
    {
        private readonly ApplicationContext _dbContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly BotMessageHandler _botMessageHandler;
        private readonly IUserService _userService;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IMessageService _messageService;
        private readonly IPostService _postService;

        public PostCreationTests(TestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _botMessageHandler = _serviceProvider.GetRequiredService<BotMessageHandler>();
            _userService = _serviceProvider.GetRequiredService<IUserService>();
            _pipelineContextService = _serviceProvider.GetRequiredService<IPipelineContextService>();
            _messageService = _serviceProvider.GetRequiredService<IMessageService>();
            _dbContext = _serviceProvider.GetRequiredService<ApplicationContext>();
            _postService = _serviceProvider.GetRequiredService<IPostService>();
        }

        private void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted(); // Delete existing data
            _dbContext.Database.EnsureCreated(); // Reinitialize fresh DB
        }

        private void SeedDatabase()
        {
            const string username = "Username";
            const string chatId = "936046085";

            _dbContext.Users.Add(new AituConnectAPI.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Role = Roles.User,
                JoinedDate = DateTime.UtcNow,
                University = string.Empty,
                Faculty = string.Empty
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Database_Cleanup()
        {
            ResetDatabase();
            var users = await _userService.GetAllAsync();
            var pipelineContexts = await _pipelineContextService.GetAllAsync();
            var messages = await _messageService.GetAllAsync();
            var posts = await _postService.GetAllAsync();
            Assert.Empty(users);
            Assert.Empty(pipelineContexts);
            Assert.Empty(messages);
            Assert.Empty(posts);
        }

        [Fact]
        public async Task Does_Entire_Pipeline_Work_Correctly()
        {
            ResetDatabase();
            SeedDatabase();
            const string title = "My first post";
            const string content = "sample text";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            // Step 2: Simulate user sending title
            var titleUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = title,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(titleUpdate);

            // Step 3: Simulate user sending content
            var contentUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = content,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(contentUpdate);

            var user = await _userService.GetByChatIdAsync(chatId);
            var posts = await _postService.GetAllByAuthorIdAsync(user.Id);
            var sortedPosts = posts.OrderByDescending(p => p.CreationDate).ToList();
            var lastPost = sortedPosts.First();

            Assert.NotNull(user);
            Assert.Equal(title, lastPost.Title);
            Assert.Equal(content, lastPost.Content);

            var pipeline = await _pipelineContextService.GetByChatIdAsync(chatId);
            Assert.Null(pipeline); // Ensure pipeline context has been deleted
        }

        [Fact]
        public async Task Create_Post_If_Not_Registered()
        {
            ResetDatabase();
            const string title = "My first post";
            const string content = "sample text";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            var user = await _userService.GetByChatIdAsync(chatId);

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);

            Assert.Null(user);
            Assert.Equal("You are not registered. Please, use /start command to register.", lastMessage.Content);
        }

        [Fact]
        public async Task Post_Creation_Interruption()
        {
            ResetDatabase();
            SeedDatabase();
            const string title = "My first post";
            const string content = "sample text";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            // Step 2: Simulate user sending /create_post again or some other command
            var createPostUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate2);

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);

            Assert.Equal("You are already in the middle of a process. Please, complete it first.", lastMessage.Content);
        }

        [Fact]
        public async Task Multiple_Posts_By_One_User()
        {
            ResetDatabase();
            SeedDatabase();
            const string titleFirstPost = "My first post";
            const string contentFirstPost = "sample text";
            const string titleSecondPost = "My second post";
            const string contentSecondPost = "another sample text";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            // Step 2: Simulate user sending title
            var titleUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = titleFirstPost,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(titleUpdate);

            // Step 3: Simulate user sending content
            var contentUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = contentFirstPost,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(contentUpdate);

            // Step 4: Simulate user sending /create_post again
            var createPostUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate2);

            // Step 5: Simulate user sending title for the second post
            var titleUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = titleSecondPost,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(titleUpdate2);

            // Step 6: Simulate user sending content for the second post
            var contentUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = contentSecondPost,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(contentUpdate2);

            var user = await _userService.GetByChatIdAsync(chatId);
            var posts = await _postService.GetAllByAuthorIdAsync(user.Id);
            var sortedPosts = posts.OrderByDescending(p => p.CreationDate).ToList();
            var firstPost = sortedPosts[1];
            var secondPost = sortedPosts.First();

            Assert.NotNull(user);
            Assert.Equal(titleFirstPost, firstPost.Title);
            Assert.Equal(contentFirstPost, firstPost.Content);

            Assert.Equal(titleSecondPost, secondPost.Title);
            Assert.Equal(contentSecondPost, secondPost.Content);
        }

        [Fact]
        public async Task Too_Long_Title()
        {
            ResetDatabase();
            SeedDatabase();
            const string title = "This is a very long title that exceeds the maximum length of 100 characters. It should not be allowed to be saved in the database.";
            const string content = "sample text";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            // Step 2: Simulate user sending title
            var titleUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = title,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(titleUpdate);

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);

            Assert.Equal("Title is too long. It has to be less than 100 characters. Please, enter a shorter title: ", lastMessage.Content);
        }

        [Fact]
        public async Task Too_Long_Content()
        {
            ResetDatabase();
            SeedDatabase();
            const string title = "My first post";
            const string content = "Song by Weezer I'm so tall, can't get over me" +
                "I'm so low, can't get under me" +
                "I must be all these things" +
                "For I just threw out the love of my dreams" +
                "He is in my eyes, he is in my ears" +
                "He is in my blood, he is in my tears" +
                "I breathe love, and see him everyday Even though my love's a world away" +
                "Oh, he's got me wondering My righteousness is crumbling" +
                "Never before have I felt this way" +
                "Know what is right, want for him to stay I must be made of steel" +
                "For I just threw out the love of my dreams" +
                "He is in my eyes, he is in my ears" +
                "He is in my blood, he is in my tears" +
                "I breathe love, and see him everyday" +
                "Even though my love's a world away" +
                "Oh, he's got me wondering" +
                "My righteousness is crumbling" +
                "Oh, he's got me wondering" +
                "My righteousness is crumbling" +
                "And I see him everyday" +
                "Even though my love's a world away" +
                "He is in my eyes, he is in my ears" +
                "He is in my blood, he is in my tears" +
                "I must be made of steel" +
                "For I just threw out the love of my dreams" +
                "Song by Weezer" +
                "Hip-hip Hip-hip Hip-hip Hip-hip" +
                "When you're on a holiday" +
                "You can't find the words to say" +
                "All the things that come to you" +
                "And I wanna feel it too" +
                "On an island in the sun" +
                "We'll be playing and having fun" +
                "And it makes me feel so fine I can't control my brain" +
                "Hip-hip Hip-hip When you're on a golden sea" +
                "You don't need no memory" +
                "Just a place to call your own" +
                "As we drift into the zone" +
                "On an island in the sun" +
                "We'll be playing and having fun" +
                "And it makes me feel so fine I can't control my brain" +
                "We'll run away together" +
                "We'll spend some time forever" +
                "We'll never feel bad anymore" +
                "Hip-hip Hip-hip Hip-hip" +
                "On an island in the sun" +
                "We'll be playing and having fun" +
                "And it makes me feel so fine I can't control my brain" +
                "We'll run away together" +
                "We'll spend some time forever" +
                "We'll never feel bad anymore" +
                "Hip-hip (Hip-hip) we'll never feel bad anymore" +
                "Hip-hip No, no (hip-hip) (Hip-hip) we'll never feel bad anymore" +
                "(Hip-hip) No, no (hip-hip) No, no (hip-hip)";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /create_post
            var createPostUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(createPostUpdate);

            // Step 2: Simulate user sending title
            var titleUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = title,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(titleUpdate);

            // Step 3: Simulate user sending content
            var contentUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = content,
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleMessageAsync(contentUpdate);

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);

            Assert.Equal("Content is too long. It has to be less than 1000 characters. Please, enter a shorter content: ", lastMessage.Content);
        }
    }
}
