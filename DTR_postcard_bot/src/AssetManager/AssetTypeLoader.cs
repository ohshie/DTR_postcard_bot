using System.Text.Json;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.AssetManager;

public class AssetTypeLoader(IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<AssetType>> Load(JsonDocument jDoc)
    {
        var jsonChannels = jDoc.RootElement.GetProperty("assetTypesByChannel").EnumerateObject();

        var assetTypes = AssembleIntoBatch(jsonChannels);

        await unitOfWork.AssetTypes.BatchAdd(assetTypes);

        return assetTypes;
    }

    private List<AssetType> AssembleIntoBatch(JsonElement.ObjectEnumerator jsonChannels)
    {
        List<AssetType> assetTypes = new();
        
        foreach (var channelAssets in jsonChannels)
        {
            foreach (var assetType in channelAssets.Value.EnumerateObject())
            {
                var newAssetType = new AssetType()
                {
                    Type = assetType.Name,
                    Text = assetType.Value.ToString()
                };

                assetTypes.Add(newAssetType);
            }
        }

        return assetTypes;
    }
}