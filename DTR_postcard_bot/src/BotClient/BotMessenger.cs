using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace DTR_postcard_bot.BotClient;

public class BotMessenger(ITelegramBotClient botClient, ILogger<BotMessenger> logger)
{
    public async Task<Message> SendTextMessage(long chatId, string text, InlineKeyboardMarkup keyboardMarkup)
    {
        var assetTextMessage = await botClient.SendTextMessageAsync(chatId: chatId,
            text: text,
            replyMarkup: keyboardMarkup);

        return assetTextMessage;
    }
    
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
            await Task.WhenAll(messagesId.Select(messageId =>
                botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId)));
        }
        catch (Exception e)
        {
            logger.LogError("Error trying to delete message in {ChatId}, produced {Exception}", chatId, e);
            throw;
        }
    }
}