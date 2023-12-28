using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Repository;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards.Buttons;

public class ButtonCreator(AssetOperator assetOperator)
{
    public async Task<InlineKeyboardButton[]> AssembleChoiceButtons(string assetType)
    {
        List<InlineKeyboardButton> buttons = new();
        
        var availableAssets = await assetOperator.GetAssetsByType(assetType);
        var counter = 0;

        foreach (var asset in availableAssets)
        {
            if (asset.Type.Type != assetType || !asset.OutputAsset) continue;
            
            ++counter;
            var button = InlineKeyboardButton.WithCallbackData(counter.ToString(), 
                $"{CallbackList.Add} {assetType} {asset.Id}");
            
            buttons.Add(button);
        }

        return buttons.ToArray();
    }
}