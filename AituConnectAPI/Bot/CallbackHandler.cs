using AituConnectAPI.Bot;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

public class CallbackHandler
{
    private readonly Dictionary<string, Func<CallbackQuery, Task>> _handlers;
    private readonly IUserService _userService;
    private readonly IPipelineContextService _pipelineContextService;
    private readonly ITelegramBotClient _botClient;
    private readonly KeyboardMarkupBuilder _keyboardMarkup;
    private readonly BotMessageSender _messageSender;

    public CallbackHandler(IServiceProvider serviceProvider, IUserService userService, IPipelineContextService pipelineContextService, ITelegramBotClient botClient, KeyboardMarkupBuilder keyboardMarkup, BotMessageSender messageSender)
    {
        _handlers = new Dictionary<string, Func<CallbackQuery, Task>>
        {
            ["choose_university"] = async (query) => await HandleChooseUniversity(query),
            ["create_post"] = async (query) => await HandleCreatePost(query)
        };
        _userService = userService;
        _userService = userService;
        _pipelineContextService = pipelineContextService;
        _botClient = botClient;
        _keyboardMarkup = keyboardMarkup;
        _messageSender = messageSender;
    }

    public async Task HandleCallbackAsync(CallbackQuery query)
    {
        if (query == null)
        {
            return;
        }

        var parts = query.Data.Split(':');
        var handlerName = parts[0];

        if (_handlers.TryGetValue(handlerName, out var handler))
        {
            await handler(query);
        }
        else
        {
            Console.WriteLine($"Unknown callback data: {query.Data}");
        }
    }

    private async Task HandleChooseUniversity(CallbackQuery query)
    {
        // Logic to process university selection
        // Extract university name (assuming format "choose_university:University Name")
        var parts = query.Data.Split(':');
        if (parts.Length < 2)
        {
            Console.WriteLine("Invalid callback data format.");
            return;
        }

        string universityName = parts[1]; // Extract name from callback data

        var chatId = query.Message.Chat.Id.ToString();
        var messageId = query.Message.MessageId;

        var context = await _pipelineContextService.GetByChatIdAsync(chatId);

        context.Content = universityName;

        await _pipelineContextService.UpdateAsync(context);

        // Remove inline buttons after selection
        await _keyboardMarkup.RemoveKeyboardAsync(_botClient, chatId, messageId);

        await _messageSender.EditTestMessageAsync(chatId, messageId, $"You've selected {universityName} as your university.");
    }

    private async Task HandleCreatePost(CallbackQuery query)
    {
        // Logic to create a post
    }
}
