using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;

public class TextAssetHandler(AssetOperator assetOperator, ITextContent textContent)
{
    public async Task<string> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);

        return BuildMessageText(allRequiredAssets);
    }

    private string BuildMessageText(IEnumerable<Asset> assets)
    {
        var messageText = textContent.RequestSomething(assets.FirstOrDefault().Type.Type);
        var counter = 0;
        
        foreach (var asset in assets)
        {
            counter++;
            messageText += $"\n{counter}. {asset.Text}";
        }

        return messageText;
    }
}