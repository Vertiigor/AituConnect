//using AituConnectAPI.Keyboards;
//using AituConnectAPI.Services.Abstractions;
//using Telegram.Bot;
//using Telegram.Bot.Types;

//namespace AituConnectAPI.Bot
//{
//    public enum CallbackType
//    {
//        ChooseUniversity,
//        EditProfile
//    }
//    public class CallbackHandler
//    {
//        private readonly Dictionary<string, Func<CallbackQuery, Task>> _handlers;
//        private readonly IPipelineContextService _pipelineContextService;
//        private readonly ITelegramBotClient _botClient;
//        private readonly KeyboardMarkupBuilder _keyboardMarkup;
//        private readonly BotMessageSender _messageSender;

//        public CallbackHandler(IServiceProvider serviceProvider, IPipelineContextService pipelineContextService, ITelegramBotClient botClient, KeyboardMarkupBuilder keyboardMarkup, BotMessageSender messageSender)
//        {
//            _handlers = new Dictionary<string, Func<CallbackQuery, Task>>
//            {
//                [CallbackType.ChooseUniversity.ToString()] = async (query) => await HandleChooseUniversity(query),
//                [CallbackType.EditProfile.ToString()] = async (query) => await HandleEditProfile(query)
//            };
//            _pipelineContextService = pipelineContextService;
//            _botClient = botClient;
//            _keyboardMarkup = keyboardMarkup;
//            _messageSender = messageSender;
//        }

//        public async Task HandleCallbackAsync(CallbackQuery query)
//        {
//            if (query == null)
//            {
//                return;
//            }

//            var parts = query.Data.Split(':');
//            var handlerName = parts[0];

//            if (_handlers.TryGetValue(handlerName, out var handler))
//            {
//                await handler(query);
//            }
//            else
//            {
//                Console.WriteLine($"Unknown callback data: {query.Data}");
//            }
//        }

//        private async Task HandleChooseUniversity(CallbackQuery query)
//        {
//            // Logic to process university selection
//            // Extract university name (assuming format "choose_university:University Name")
//            var parts = query.Data.Split(':');
//            if (parts.Length < 2)
//            {
//                Console.WriteLine("Invalid callback data format.");
//                return;
//            }

//            string universityName = parts[1]; // Extract name from callback data

//            var chatId = query.Message.Chat.Id.ToString();
//            var messageId = query.Message.Id;

//            var context = await _pipelineContextService.GetByChatIdAsync(chatId);

//            context.Content = universityName;

//            await _pipelineContextService.UpdateAsync(context);

//            // Remove inline buttons after selection
//            await _keyboardMarkup.RemoveKeyboardAsync(_botClient, chatId, messageId);

//            await _messageSender.EditTestMessageAsync(chatId, messageId, $"You've selected {universityName} as your university.");
//        }

//        private async Task HandleEditProfile(CallbackQuery query)
//        {
//            // Logic to process profile editing
//            // Extract option (assuming format "edit_profile:Option")
//            var parts = query.Data.Split(':');
//            if (parts.Length < 2)
//            {
//                Console.WriteLine("Invalid callback data format.");
//                return;
//            }

//            string option = parts[1]; // Extract option from callback data

//            var chatId = query.Message.Chat.Id.ToString();
//            var messageId = query.Message.MessageId;

//            var context = await _pipelineContextService.GetByChatIdAsync(chatId);

//            context.Content = option;

//            await _pipelineContextService.UpdateAsync(context);

//            // Remove inline buttons after selection
//            await _keyboardMarkup.RemoveKeyboardAsync(_botClient, chatId, messageId);

//            await _messageSender.EditTestMessageAsync(chatId, messageId, $"You've selected {option} as your profile option.");
//        }
//    }
//}