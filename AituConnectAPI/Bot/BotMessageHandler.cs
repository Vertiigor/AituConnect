using AituConnectAPI.Pipelines.Registration;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

public class BotMessageHandler
{
    private readonly CommandDispatcher _dispatcher;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITelegramBotClient _botClient;
    private readonly RegistrationPipeline _registrationPipeline;
    private readonly IPipelineContextService _pipelineContextService;
    private readonly IUserService _userService;

    public BotMessageHandler(CommandDispatcher dispatcher, IServiceScopeFactory scopeFactory, ITelegramBotClient botClient, RegistrationPipeline registrationPipeline, IPipelineContextService pipelineContextService, IUserService userService)
    {
        _dispatcher = dispatcher;
        _scopeFactory = scopeFactory;
        _botClient = botClient;
        _registrationPipeline = registrationPipeline;
        _pipelineContextService = pipelineContextService;
        _userService = userService;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        using var scope = _scopeFactory.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<CommandDispatcher>();
        await dispatcher.DispatchAsync(update);
    }
    public async Task HandleMessageAsync(Update update)
    {
        if (update.Message == null) return;

        var chatId = update.Message.Chat.Id.ToString();
        var user = await _userService.GetByChatIdAsync(chatId);

        if (user == null) return;

        var context = await _pipelineContextService.GetByChatIdAsync(chatId);

        if (context == null) return;

        // Ignore commands
        if (update.Message.Text.StartsWith("/"))
        {
            return;
        }

        // Update the context with the user's input
        context.Content = update.Message.Text;

        // Save the updated context
        await _pipelineContextService.UpdateAsync(context);


        await _registrationPipeline.ExecuteAsync(context); // Execute the current step in the pipeline
        await _registrationPipeline.ExecuteAsync(context); // Execute the next step in the pipeline
    }

}
