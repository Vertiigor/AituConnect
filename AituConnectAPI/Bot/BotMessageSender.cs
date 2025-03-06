using Telegram.Bot;

namespace AituConnectAPI.Bot
{
    public class BotMessageSender
    {
        private readonly ITelegramBotClient _botClient;

        public BotMessageSender(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendTextMessageAsync(string chatId, string text)
        {
            await _botClient.SendMessage(chatId, text);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup replyMarkup)
        {
            await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove replyMarkup)
        {
            await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
        }

        public async Task EditTestMessageAsync(string chatId, int messageId, string text)
        {
            await _botClient.EditMessageText(chatId, messageId, text);
        }
    }
}
