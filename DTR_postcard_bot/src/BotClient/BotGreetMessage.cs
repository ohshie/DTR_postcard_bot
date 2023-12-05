using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.TextContent;

namespace DTR_postcard_bot.BotClient;

public class BotGreetMessage(ITelegramBotClient botClient, ITextContent textContent, GreetingsKeyboard greetingsKeyboard)
{
    public async Task Send(Message message)
    {
        if (message.Text!.StartsWith("/start"))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                text: textContent.GreetingsMessage(), 
                replyMarkup: greetingsKeyboard.CreateKeyboard());
        }
    }
}