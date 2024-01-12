using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.DAL;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BotClient;

public class BotGreetMessage(ITelegramBotClient botClient, 
    TextContent textContent, 
    CardCreationKeyboard cardCreationKeyboard, IUnitOfWork unitOfWork)
{
    public async Task Send(Message message)
    {
        if (message.Text!.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                text: await textContent.GetRequiredText("greetingsMessage"), 
                replyMarkup: cardCreationKeyboard.CreateKeyboard());

            await CreateStatRecord(message.From!.Id, message.From.Username);
        }
    }

    public async Task Send(long chatId)
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            text: await textContent.GetRequiredText("greetingsMessage"),
            replyMarkup: cardCreationKeyboard.CreateKeyboard());

        await CreateStatRecord(chatId, string.Empty);
    }
    
    private async Task CreateStatRecord(long userId, string? name = null)
    {
        var stat = new Stat()
        {
            UserId = userId,
            UserName = string.IsNullOrEmpty(name) ? "Anonymous" : name,
            CreatedCards = 0,
            DroppedCards = 0
        };
            
        await unitOfWork.Stats.Add(stat);
    }
}