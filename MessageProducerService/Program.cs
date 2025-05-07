using MessageProducerService.Bot;
using MessageProducerService.Consumers;
using MessageProducerService.Data;
using MessageProducerService.Data.Connections.RabbitMq;
using MessageProducerService.Data.Settings;
using MessageProducerService.Keyboards;
using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;
using MessageProducerService.Repositories.Implementations;
using MessageProducerService.Services;
using MessageProducerService.Services.Abstractions;
using MessageProducerService.Services.Implementations;
using MessageProducerService.StepHandlers.Abstractions;
using MessageProducerService.StepHandlers.Implementations.ProfileEditing;
using MessageProducerService.StepHandlers.Implementations.Registration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace MessageProducerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseContext") ?? throw new InvalidOperationException("Connection string 'BookStoreContext' not found.")));

        var botApiToken = builder.Configuration["BotConnection:ApiToken"]
            ?? throw new InvalidOperationException("API token 'BotConnection:ApiToken' not found.");

        // Configure Identity to use the custom ApplicationUser model
        builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();

        builder.AddServiceDefaults();

        // Add services to the container.

        // Register RabbitMQ connection
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botApiToken));
        builder.Services.AddScoped<BotMessageSender>();
        builder.Services.AddScoped<KeyboardMarkupBuilder>();

        builder.Services.AddScoped<StepHandler, StartCommandHandler>();
        builder.Services.AddScoped<StepHandler, MajorStepHandler>();
        builder.Services.AddScoped<StepHandler, UniversityStepHandler>();

        builder.Services.AddScoped<StepHandler, EditProfileCommandHandler>();
        builder.Services.AddScoped<StepHandler, EditMajorStepHandler>();
        builder.Services.AddScoped<StepHandler, EditUniversityStepHandler>();
        builder.Services.AddScoped<StepHandler, EditInputStepHandler>();

        // Register the message consumer
        builder.Services.AddHostedService<UserQueueConsumer>();

        builder.Services.AddScoped<HandlerRouter>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
