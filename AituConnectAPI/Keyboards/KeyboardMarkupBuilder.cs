using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

public class KeyboardMarkupBuilder
{
    public InlineKeyboardButton InitializeInlineKeyboardButton(string text, string callbackData)
    {
        return new InlineKeyboardButton
        {
            Text = text,
            CallbackData = callbackData
        };
    }

    public InlineKeyboardMarkup InitializeInlineKeyboardMarkup(List<InlineKeyboardButton> buttons, int buttonsPerRow = 2)
    {
        return new InlineKeyboardMarkup(buttons.Chunk(buttonsPerRow));
    }

    public async Task RemoveKeyboardAsync(ITelegramBotClient botClient, string chatId, int messageId)
    {
        await botClient.EditMessageReplyMarkup(
            chatId: chatId,
            messageId: messageId,
            replyMarkup: null // This removes the inline buttons
        );
    }
}