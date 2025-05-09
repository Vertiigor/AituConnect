using Telegram.Bot;

namespace MessageProducerService.Bot
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
            var message = await _botClient.SendMessage(chatId, text);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup replyMarkup)
        {
            var message = await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
        }

        public async Task SendTextMessageAsync(string chatId, string text, Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove replyMarkup)
        {
            var message = await _botClient.SendMessage(chatId, text, replyMarkup: replyMarkup);
        }

        public async Task EditTestMessageAsync(string chatId, int messageId, string text)
        {
            await _botClient.EditMessageText(chatId, messageId, text);
        }
    }
}
