using AituConnectAPI.Bot;
using MessageListenerService.Bot;
using MessageListenerService.Commands;
using MessageListenerService.Data.Connections.RabbitMq;
using MessageListenerService.Data.Connections.Redis;
using MessageListenerService.Data.Settings;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Producers.Implementations;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;
using MessageListenerService.StepHandlers.Implementations.Registration;
using Telegram.Bot;

namespace MessageListenerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var botApiToken = builder.Configuration["BotConnection:ApiToken"]
            ?? throw new InvalidOperationException("API token 'BotConnection:ApiToken' not found.");

        builder.AddServiceDefaults();

        // Add services to the container.

        // Register the Redis connection
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
        builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        // Register RabbitMQ connection
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.AddSingleton<IRedisConnection, RedisConnection>();

        // Register the Telegram Bot client
        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botApiToken));
        builder.Services.AddScoped<CommandDispatcher>();
        builder.Services.AddScoped<BotMessageHandler>();
        builder.Services.AddSingleton<BotClient>();
        builder.Services.AddScoped<HandlerRouter>();

        builder.Services.AddScoped<StepHandler, MajorStepHandler>();
        builder.Services.AddScoped<StepHandler, UniversityStepHandler>();

        // Register the command handler
        builder.Services.AddScoped<ICommand, StartCommand>();

        // Register the services
        builder.Services.AddScoped<UserSessionService>();
        builder.Services.AddTransient<IMessageProducer, Producer>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        var botHandler = app.Services.GetRequiredService<BotClient>();
        botHandler.StartReceiving();

        app.Run();
    }
}
