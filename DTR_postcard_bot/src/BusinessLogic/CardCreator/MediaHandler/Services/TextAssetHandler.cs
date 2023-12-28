using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public class TextAssetHandler(AssetOperator assetOperator, TextContent textContent)
{
    public async Task<string> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);

        return await BuildMessageText(allRequiredAssets);
    }

    private async Task<string> BuildMessageText(IEnumerable<Asset> assets)
    {
        var messageText = await textContent.GetRequiredText("requestSomething", assets.FirstOrDefault().Type.Text);
        var counter = 0;
        
        foreach (var asset in assets)
        {
            counter++;
            messageText += $"\n{counter}. {asset.Text}";
        }

        return messageText;
    }
}