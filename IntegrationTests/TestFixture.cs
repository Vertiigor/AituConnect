using AituConnectAPI.Bot;
using AituConnectAPI.Commands;
using AituConnectAPI.Data;
using AituConnectAPI.Keyboards;
using AituConnectAPI.Pipelines.Editing.Profile;
using AituConnectAPI.Pipelines.PostCreation;
using AituConnectAPI.Pipelines.Registration;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Repositories.Implementations;
using AituConnectAPI.Services.Abstractions;
using AituConnectAPI.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace IntegrationTests
{
    public class TestFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }
        private readonly string _databaseName;

        public TestFixture()
        {
            // Generate a unique database name per test session
            _databaseName = Guid.NewGuid().ToString();

            var serviceCollection = new ServiceCollection()
                .AddDbContext<ApplicationContext>(options =>
                    options.UseInMemoryDatabase(_databaseName)) // Unique DB per test
                .AddScoped<IUserService, UserService>()
                .AddScoped<IPostService, PostService>()
                .AddScoped<IPipelineContextService, PipelineContextService>()
                .AddScoped<IMessageService, MessageService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPostRepository, PostRepository>()
                .AddScoped<IPipelineContextRepository, PipelineContextRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<CallbackHandler>()
                .AddScoped<PipelineHandler>()
                .AddScoped<KeyboardMarkupBuilder>()
                .AddScoped<BotMessageSender>()
                .AddScoped<ICommand, StartCommand>()
                .AddScoped<ICommand, CreatePostCommand>()
                .AddScoped<ICommand, EditProfileCommand>()
                .AddScoped<BotMessageHandler>()
                .AddScoped<CommandDispatcher>()
                .AddSingleton<BotClient>()
                .AddScoped<RegistrationPipeline>()
                .AddScoped<UniversityStep>()
                .AddScoped<FacultyStep>()
                .AddScoped<CongratulationStep>()
                .AddScoped<PostCreationPipeline>()
                .AddScoped<TitleStep>()
                .AddScoped<ContentStep>()
                .AddScoped<ProfileEditingPipeline>()
                .AddScoped<OptionStep>()
                .AddScoped<UniversityEditingStep>()
                .AddScoped<FacultyEditingStep>()
                .AddSingleton<ITelegramBotClient, TelegramBotClient>(_ =>
                    new TelegramBotClient("7498941247:AAEdY_ab9r2cHZUr_w_6K5wO2JF94isWF5o")) // Mock token
                .BuildServiceProvider();

            ServiceProvider = serviceCollection;
        }

        public void Dispose()
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            dbContext.Database.EnsureDeleted(); // Clean up the database after each test
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }

}