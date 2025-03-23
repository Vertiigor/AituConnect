using AituConnectAPI.Bot;
using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace IntegrationTests
{
    public class ProfileEditingTests : IClassFixture<TestFixture>
    {
        private readonly ApplicationContext _dbContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly BotMessageHandler _botMessageHandler;
        private readonly IUserService _userService;
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IMessageService _messageService;

        public ProfileEditingTests(TestFixture fixture)
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

        private void SeedDatabase()
        {
            const string username = "Username";
            const string chatId = "936046085";
            const string university = "University of Utah";
            const string faculty = "Computer Science";

            _dbContext.Users.Add(new AituConnectAPI.Models.User
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Role = Roles.User,
                JoinedDate = DateTime.UtcNow,
                University = university,
                Faculty = faculty
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
            Assert.Empty(users);
            Assert.Empty(pipelineContexts);
            Assert.Empty(messages);
        }

        [Fact]
        public async Task Choose_University_Option()
        {
            ResetDatabase();
            SeedDatabase();

            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /start
            var editProfileUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/edit_profile",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(editProfileUpdate);

            // Step 2: Fetch the pipeline context from DB (ensure it was created)
            var pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            Assert.NotNull(pipelineContext);
            Assert.Equal(PipelineStepType.ChoosingOption.ToString(), pipelineContext.CurrentStep.ToString()); // Ensure step is correctly set

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);
            Assert.NotNull(lastMessage);

            // Step 3: Simulate user selecting an option via inline keyboard
            var callbackQuery = new CallbackQuery
            {
                Id = "callback_123",
                From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false },
                Data = $"{CallbackType.EditProfile.ToString()}:ChoosingUniversity",
                Message = new Telegram.Bot.Types.Message
                {
                    Id = Convert.ToInt32(lastMessage.MessageId),
                    Text = "Choose your university",
                    Chat = new Chat { Id = Convert.ToInt32(chatId) }
                }
            };

            var optionUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Chat = new Chat { Id = Convert.ToInt32(chatId) },
                    Text = $"{CallbackType.EditProfile.ToString()}:ChoosingUniversity",
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }

                },
                CallbackQuery = callbackQuery
            };

            await _botMessageHandler.HandleMessageAsync(optionUpdate);

            pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            var user = await _userService.GetByChatIdAsync(chatId);

            Assert.NotNull(pipelineContext);
            Assert.Equal(PipelineStepType.ChoosingUniversity.ToString(), pipelineContext.CurrentStep.ToString());
            

            lastMessage = await _messageService.GetLastByChatIdAsync(chatId);
            Assert.NotNull(lastMessage);

            // Step 4: Simulate user selecting a university via inline keyboard
            var callbackUniversityQuery = new CallbackQuery
            {
                Id = "callback_123",
                From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false },
                Data = $"{CallbackType.ChooseUniversity.ToString()}:MIT",
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
                    Text = $"{CallbackType.ChooseUniversity.ToString()}:MIT",
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }

                },
                CallbackQuery = callbackUniversityQuery
            };

            await _botMessageHandler.HandleMessageAsync(universityUpdate);

            pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            user = await _userService.GetByChatIdAsync(chatId);

            Assert.Null(pipelineContext);
            Assert.Equal("MIT", user.University);
        }

        [Fact]
        public async Task Choose_Faculty_Option()
        {
            ResetDatabase();
            SeedDatabase();

            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /edit_profile
            var editProfileUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/edit_profile",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(editProfileUpdate);

            // Step 2: Fetch the pipeline context from DB (ensure it was created)
            var pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);

            Assert.NotNull(pipelineContext);
            Assert.Equal(PipelineStepType.ChoosingOption.ToString(), pipelineContext.CurrentStep.ToString()); // Ensure step is correctly set

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);
            Assert.NotNull(lastMessage);

            // Step 3: Simulate user selecting an option via inline keyboard
            var callbackQuery = new CallbackQuery
            {
                Id = "callback_123",
                From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false },
                Data = $"{CallbackType.EditProfile.ToString()}:ChoosingFaculty",
                Message = new Telegram.Bot.Types.Message
                {
                    Id = Convert.ToInt32(lastMessage.MessageId),
                    Text = "Choose your faculty",
                    Chat = new Chat { Id = Convert.ToInt32(chatId) }
                }
            };
            var optionUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Chat = new Chat { Id = Convert.ToInt32(chatId) },
                    Text = $"{CallbackType.EditProfile.ToString()}:ChoosingFaculty",
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }
                },
                CallbackQuery = callbackQuery
            };

            await _botMessageHandler.HandleMessageAsync(optionUpdate);

            pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            var user = await _userService.GetByChatIdAsync(chatId);

            Assert.NotNull(pipelineContext);
            Assert.Equal(PipelineStepType.ChoosingFaculty.ToString(), pipelineContext.CurrentStep.ToString());

            lastMessage = await _messageService.GetLastByChatIdAsync(chatId);
            Assert.NotNull(lastMessage);

            // Step 4: Simulate user typing a faculty name
            var facultyUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Chat = new Chat { Id = Convert.ToInt32(chatId) },
                    Text = "Linguistic philosophy",
                    From = new Telegram.Bot.Types.User
                    {
                        Id = 12345,
                        IsBot = false
                    }
                }
            };

            await _botMessageHandler.HandleMessageAsync(facultyUpdate);

            pipelineContext = await _pipelineContextService.GetByChatIdAsync(chatId);
            user = await _userService.GetByChatIdAsync(chatId);

            Assert.Null(pipelineContext);
            Assert.Equal("Linguistic philosophy", user.Faculty);
        }

        [Fact]
        public async Task Profile_Editing_Interruption()
        {
            ResetDatabase();
            SeedDatabase();
            const string username = "Username";
            const string chatId = "936046085";

            // Step 1: Simulate user sending /start
            var editProfileUpdate = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/edit_profile",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(editProfileUpdate);

            // Step 2: Simulate user sending /edit_profile again or some other command
            var editProfileUpdate2 = new Update
            {
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/create_post",
                    Chat = new Chat { Id = Convert.ToInt32(chatId), Username = username },
                    From = new Telegram.Bot.Types.User { Id = 12345, IsBot = false }
                }
            };

            await _botMessageHandler.HandleUpdateAsync(editProfileUpdate2);

            var lastMessage = await _messageService.GetLastByChatIdAsync(chatId);

            Assert.Equal("You are already in the middle of a process. Please, complete it first.", lastMessage.Content);
        }
    }
}
