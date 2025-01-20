using AituConnectAPI.Bot;
using AituConnectAPI.Commands;
using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Repositories.Implementations;
using AituConnectAPI.Services.Abstractions;
using AituConnectAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace AituConnectAPI;

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
        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botApiToken));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<ICommand, StartCommand>();
        builder.Services.AddScoped<BotMessageHandler>();
        builder.Services.AddScoped<CommandDispatcher>();
        builder.Services.AddSingleton<BotClient>();


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        var botHandler = app.Services.GetRequiredService<BotClient>();
        botHandler.StartReceiving();

        app.Run();
    }
}
