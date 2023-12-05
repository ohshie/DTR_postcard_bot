namespace DTR_postcard_bot.BotClient;

public class BotMessenger(ITelegramBotClient botClient, Logger<BotMessenger> logger)
{
    public async Task UpdateMessageAsync(int chatId, string text, int messageId)
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

    public async Task DeleteMessageAsync(int chatId, int messageId)
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
}