using MessageProducerService.Consumers;
using MessageProducerService.Data.Connections.RabbitMq;
using MessageProducerService.Data.Settings;

namespace MessageProducerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Add services to the container.

        // Register RabbitMQ connection
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        // Register the message consumer
        builder.Services.AddHostedService<UserQueueConsumer>();

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
