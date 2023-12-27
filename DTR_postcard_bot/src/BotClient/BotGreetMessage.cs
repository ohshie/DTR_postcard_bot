using DTR_postcard_bot.BotClient.Keyboards;

namespace DTR_postcard_bot.BotClient;

public class BotGreetMessage(ITelegramBotClient botClient, 
    TextContent textContent, 
    CardCreationKeyboard cardCreationKeyboard, BotMessenger botMessenger)
{
    public async Task Send(Message message)
    {
        if (message.Text!.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                text: await textContent.GetRequiredText("greetingsMessage"), 
                replyMarkup: cardCreationKeyboard.CreateKeyboard());
        }
    }

    public async Task Send(long chatId)
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            text: await textContent.GetRequiredText("greetingsMessage"),
            replyMarkup: cardCreationKeyboard.CreateKeyboard());
    }
}