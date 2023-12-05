using DTR_postcard_bot.DataLayer.Repository;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards;

public class BgKeyboard(AssetOperator assetOperator)
{
    public async Task<InlineKeyboardMarkup> CreateKeyboard()
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            await AssembleChoiceButtons(),
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Я передумал, давай начнем заново", 
                    callbackData: CallbackList.StartCardCreation)
            }
        });

        return keyboard;
    }

    private async Task<InlineKeyboardButton[]> AssembleChoiceButtons()
    {
        List<InlineKeyboardButton> buttons = new();
        
        var availableAssets = await assetOperator.GetAllAssets();
        var counter = 0;
        
        foreach (var asset in availableAssets)
        {
            ++counter;
            var button = InlineKeyboardButton.WithCallbackData(counter.ToString(), 
                $"{CallbackList.BgChoice}{counter}");
            
            buttons.Add(button);
        }

        return buttons.ToArray();
    }
}