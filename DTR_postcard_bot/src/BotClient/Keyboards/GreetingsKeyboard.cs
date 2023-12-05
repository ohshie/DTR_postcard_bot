using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards;

public class GreetingsKeyboard
{
    public InlineKeyboardMarkup CreateKeyboard()
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Поехали!", callbackData: CallbackList.StartCardCreation)
            }
        });

        return keyboard;
    }
}