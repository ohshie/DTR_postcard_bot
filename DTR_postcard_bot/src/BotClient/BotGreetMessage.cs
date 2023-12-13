using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.TextContent;

namespace DTR_postcard_bot.BotClient;

public class BotGreetMessage(ITelegramBotClient botClient, 
    ITextContent textContent, 
    CardCreationKeyboard cardCreationKeyboard, BotMessenger botMessenger)
{
    public async Task Send(Message message)
    {
        if (message.Text!.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                text: textContent.GreetingsMessage(), 
                replyMarkup: cardCreationKeyboard.CreateKeyboard());
        }
    }

    public async Task Send(long chatId)
    {
        await botClient.SendTextMessageAsync(chatId: chatId,
            textContent.GreetingsMessage(),
            replyMarkup: cardCreationKeyboard.CreateKeyboard());
    }
}