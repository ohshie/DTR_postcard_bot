using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards;

public class CardCreationKeyboard
{
    public InlineKeyboardMarkup CreateKeyboard(bool firstTime = true)
    {
        InlineKeyboardMarkup keyboard;
        if (firstTime)
        {
            keyboard = new InlineKeyboardMarkup((new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Поехали!",
                        callbackData: CallbackList.StartCardCreation)
                }
            }));
        }
        else
        {
            keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Хочу еще!",
                        callbackData: CallbackList.CreateNew)
                }
            });
        }
       

        return keyboard;
    }
}