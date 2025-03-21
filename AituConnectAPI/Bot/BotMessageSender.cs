using AituConnectAPI.Models;
using AituConnectAPI.Services.Abstractions;
using Telegram.Bot;

namespace AituConnectAPI.Bot
{
    public class BotMessageSender
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMessageService _messageService;

        public BotMessageSender(ITelegramBotClient botClient, IMessageService messageService)
        {
            _botClient = botClient;
            _messageService = messageService;
        }

        private async Task StoreMessage(string chatId, string text, Telegram.Bot.Types.Message message)
        {
            await _messageService.AddAsync(new Message
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                MessageId = Convert.ToString(message.MessageId),
                Content = text,
                SentTime = DateTime.UtcNow
            });
        }

        public async Task SendTextMessageAsync(string chatId, string text)
        {
            var message = await _botClient.SendMessage(chatId, text);
            await StoreMessage(chatId, text, message);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup replyMarkup)
        {
            var message = await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
            await StoreMessage(chatId, text, message);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove replyMarkup)
        {
            var message = await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
            await StoreMessage(chatId, text, message);
        }

        public async Task EditTestMessageAsync(string chatId, int messageId, string text)
        {
            await _botClient.EditMessageText(chatId, messageId, text);
        }
    }
}
