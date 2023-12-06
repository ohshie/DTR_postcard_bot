using DTR_postcard_bot.BotClient.Keyboards.Buttons;
using DTR_postcard_bot.DataLayer.Repository;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards;

public class AssetChoiceKeyboard(ButtonCreator buttonCreator)
{
    public async Task<InlineKeyboardMarkup> CreateKeyboard(string assetType)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            await buttonCreator.AssembleChoiceButtons(assetType),
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Я передумал, давай начнем заново", 
                    callbackData: CallbackList.Cancel)
            }
        });

        return keyboard;
    }
}