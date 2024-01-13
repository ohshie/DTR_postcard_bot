using DTR_postcard_bot.DAL.UoW.IUoW;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards.Buttons;

public class ButtonCreator(IUnitOfWork unitOfWork)
{
    public async Task<InlineKeyboardButton[]> AssembleChoiceButtons(string assetType)
    {
        List<InlineKeyboardButton> buttons = new();
        
        var availableAssets = await unitOfWork.Assets.GetByType(assetType);
       
        var counter = 0;

        foreach (var asset in availableAssets)
        {
            if (asset!.Type.Type != assetType || !asset.OutputAsset) continue;
            
            ++counter;
            var button = InlineKeyboardButton.WithCallbackData(counter.ToString(), 
                $"{CallbackList.Add} {assetType} {asset.Id}");
            
            buttons.Add(button);
        }

        return buttons.ToArray();
    }
}