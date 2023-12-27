using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BotClient;

public class BotGreetMessage(ITelegramBotClient botClient, 
    TextContent textContent, 
    CardCreationKeyboard cardCreationKeyboard, StatOperator statOperator)
{
    public async Task Send(Message message)
    {
        if (message.Text!.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                text: await textContent.GetRequiredText("greetingsMessage"), 
                replyMarkup: cardCreationKeyboard.CreateKeyboard());

            await statOperator.RegisterUser(message.From.Id, message.From.Username);
        }
    }

    public async Task Send(long chatId)
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            text: await textContent.GetRequiredText("greetingsMessage"),
            replyMarkup: cardCreationKeyboard.CreateKeyboard());
        
        await statOperator.RegisterUser(chatId, string.Empty);
    }
}