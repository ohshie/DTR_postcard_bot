using System.Text;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

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
        var sb = new StringBuilder();
        var assetsMap = new HashSet<Asset>(assets);
        
        var messageText = await textContent.GetRequiredText("requestSomething", assetsMap.FirstOrDefault()!.Type.Text);
        var counter = 0;

        sb.Append(messageText);
        
        foreach (var asset in assetsMap)
        {
            counter++;
            sb.Append($"\n{counter}. {asset.Text}");
        }

        return sb.ToString();
    }
}