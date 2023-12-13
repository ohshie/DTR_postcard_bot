using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace DTR_postcard_bot.BotClient;

public class BotMessenger(ITelegramBotClient botClient, ILogger<BotMessenger> logger)
{
    public async Task<List<Message>> SendNewMediaGroupMessage(long chatId, string text, InlineKeyboardMarkup keyboardMarkup, IEnumerable<InputMediaPhoto> media)
    {
        List<Message> combinedMediaAndTextMessages = new();

        var mediaMessageArray = await botClient.SendMediaGroupAsync(chatId: chatId, media: media);
        
        var message = await botClient.SendTextMessageAsync(chatId: chatId,
            text: text,
            replyMarkup: keyboardMarkup);
        
        combinedMediaAndTextMessages.AddRange(mediaMessageArray);
        combinedMediaAndTextMessages.Add(message);

        return combinedMediaAndTextMessages;
    }

    public async Task SendNewMediaMessage(long chatId, string text, InlineKeyboardMarkup keyboardMarkup,
        string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await botClient.SendPhotoAsync(chatId,
                photo: InputFile.FromStream(stream),
                caption: text,
                replyMarkup: keyboardMarkup);
        }
    }
    
    public async Task UpdateMessageAsync(long chatId, string text, int messageId)
    {
        try
        {
            var message = await botClient.EditMessageTextAsync(text: text, chatId: chatId, messageId: messageId);
        }
        catch (Exception e)
        {
            logger.LogError("Error trying to update message in {ChatId}, {MessageId}, produced {Exception}", chatId, messageId, e);
            throw;
        }
    }
    
    public async Task UpdateMessageAsync(long chatId, string text, int messageId, InlineKeyboardMarkup keyboardMarkup)
    {
        try
        {
            await botClient.EditMessageTextAsync(text: text, chatId: chatId, messageId: messageId, replyMarkup: keyboardMarkup);
        }
        catch (Exception e)
        {
            logger.LogError("Error trying to update message in {ChatId}, {MessageId}, produced {Exception}", chatId, messageId, e);
            throw;
        }
    }

    public async Task DeleteMessageAsync(long chatId, int messageId)
    {
        try
        {
            await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId);
        }
        catch (Exception e)
        {
            logger.LogError("Error trying to delete message in {ChatId}, {MessageId}, produced {Exception}", chatId, messageId, e);
            throw;
        }
    }

    public async Task DeleteMessageRangeAsync(long chatId, List<int> messagesId)
    {
        try
        {
            foreach (var messageId in messagesId)
            {
                await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error trying to delete message in {ChatId}, produced {Exception}", chatId, e);
            throw;
        }
    }
}