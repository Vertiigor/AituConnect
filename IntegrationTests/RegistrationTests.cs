using AituConnectAPI.Bot;
using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace IntegrationTests
{
    public class RegistrationTests : IClassFixture<TestFixture>
    {
        private readonly ApplicationContext _dbContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly BotMessageHandler _botMessageHandler;
        private readonly IUserService _userService;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IMessageService _messageService;

        public RegistrationTests(TestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _botMessageHandler = _serviceProvider.GetRequiredService<BotMessageHandler>();
            _userService = _serviceProvider.GetRequiredService<IUserService>();
            _pipelineContextService = _serviceProvider.GetRequiredService<IPipelineContextService>();
            _messageService = _serviceProvider.GetRequiredService<IMessageService>();
            _dbContext = _serviceProvider.GetRequiredService<ApplicationContext>();
        }

        private void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted(); // Delete existing data
            _dbContext.Database.EnsureCreated(); // Reinitialize fresh DB
        }

        [Fact]
        public async Task Database_Cleanup()
        {
            ResetDatabase();
            var users = await _userService.GetAllAsync();
            var pipelineContexts = await _pipelineContextService.GetAllAsync();
            var messages = await _messageService.GetAllAsync();
            Assert.Empty(users);
            Assert.Empty(pipelineContexts);
            Assert.Empty(messages);
        }

        [Fact]
        public async Task Registration_Interruption()
        {
            ResetDatabase();
            const string chatId = "936046085";
            const string username = "Username";
            
            // Step 1: Simulate user sending /start
            var startUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/start",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(startUpdate);
            
            // Step 2: Simulate user sending /start again
            var startUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/start",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(startUpdate2);
            
            var messages = await _messageService.GetAllByChatIdAsync(chatId);
            var sortedMessages = messages.OrderByDescending(m => m.SentTime).ToList();
            var lastMessage = sortedMessages.First();
            
            Assert.Equal("You are already in the middle of a process. Please, complete it first.", lastMessage.Content);
        }

        [Fact]
        public async Task Does_Entire_Pipeline_Work_Correctly()
        {
            ResetDatabase();
            const string university = "MIT";
            const string faculty = "Computer Science";
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /start
            var startUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/start",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(startUpdate);

            // Step 2: Fetch the pipeline context from DB (ensure it was created)
            var pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            Assert.NotNull(pipelineContext);
            Assert.Equal("ChoosingUniversity", pipelineContext.CurrentStep.ToString()); // Ensure step is correctly set


            var messages = await _messageService.GetAllByChatIdAsync(chatId);
            var sortedMessages = messages.OrderByDescending(m => m.SentTime).ToList();
            var lastMessage = messages.First();

            // Step 3: Simulate user selecting a university via inline keyboard
            var callbackQuery = new CallbackQuery
            {
                Id = "callback_123",
                From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false },
                Data = $"{CallbackType.ChooseUniversity.ToString()}:{university}",
                Message = new Telegram.Bot.Types.Message
                {
                    Id = Convert.ToInt32(lastMessage.MessageId),
                    Text = "Choose your university",
                    Chat = new Chat { Id = 936046085 },

                }
            };

            var universityUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Chat = new Chat { Id = Convert.ToInt32(chatId) },
                    Text = $"{CallbackType.ChooseUniversity.ToString()}:{university}",
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }

                },
                CallbackQuery = callbackQuery
            };

            await _botMessageHandler.HandleMessageAsync(universityUpdate);

            // Step 4:  Simulate user typing a faculty name
            var facultyUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Chat = new Chat { Id = Convert.ToInt32(chatId) },
                    Text = faculty,
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }
                }
            };

            await _botMessageHandler.HandleMessageAsync(facultyUpdate);

            // Step 5: Verify that the user’s university has been updated in DB
            var user = await _userService.GetByChatIdAsync(chatId);
            Assert.NotNull(user);
            Assert.Equal(university, user.University);

            var pipeline = await _pipelineContextService.GetByChatIdAsync(chatId);
            Assert.Null(pipeline); // Ensure pipeline context has been deleted
        }

        [Fact]
        public async Task Duplicate_Registration_Attempt()
        {
            ResetDatabase();
            const string university = "MIT";
            const string faculty = "Computer Science";
            const string username = "Username";
            const string chatId = "936046085";

            var existingUser = new AituConnectAPI.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                UserName = username,
                University = university,
                Faculty = faculty,
                Role = Roles.User
            };

            await _userService.AddAsync(existingUser);

            // Step 1: Simulate user sending /start
            var startUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/start",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(startUpdate);

            var messages = await _messageService.GetAllByChatIdAsync(chatId);
            var sortedMessages = messages.OrderByDescending(m => m.SentTime).ToList();
            var lastMessage = messages.First();

            Assert.Equal("You are already registered!", lastMessage.Content);
        }
    }
}
