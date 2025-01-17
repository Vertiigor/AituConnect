using Telegram.Bot.Types;

public class BotMessageHandler
{
    private readonly CommandDispatcher _dispatcher;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotMessageHandler(CommandDispatcher dispatcher, IServiceScopeFactory scopeFactory)
    {
        _dispatcher = dispatcher;
        _scopeFactory = scopeFactory;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        using var scope = _scopeFactory.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<CommandDispatcher>();
        await dispatcher.DispatchAsync(update);
    }
}
