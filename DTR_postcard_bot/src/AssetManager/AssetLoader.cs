using System.Text.Json;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.AssetManager;

public class AssetLoader(AssetOperator assetOperator, 
    AssetTypeOperator assetTypeOperator)
{
    public async Task Load(JsonDocument jDoc)
    {
        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        
        var assets = AssembleIntoBatch(jDoc, assetTypes.ToArray());

        await assetOperator.AddBatchAssets(assets);
    }

    private List<Asset> AssembleIntoBatch(JsonDocument jDoc, 
        AssetType[] assetTypes)
    {
        List<Asset> assets = new();
        
        foreach (var type in jDoc.RootElement.GetProperty("mediaLinks").EnumerateArray())
        {
            var asset = new Asset
            {
                Channel = type.GetProperty("channel").ToString(),
                Type = assetTypes.FirstOrDefault(at => at.Type == type.GetProperty("type").ToString()),
                FileName = type.GetProperty("filePath").ToString()
            };

            type.TryGetProperty("text", out var text);
            asset.Text = text.ToString();

            asset.FileUrl = string.IsNullOrEmpty(type.GetProperty("fileUrl").ToString())
                ? Helpers.PathBuilder("assets", asset.Type!.Type, asset.FileName)
                : type.GetProperty("fileUrl").ToString();

            asset.OutputAsset = type.GetProperty("outputAsset").GetBoolean();
            asset.DisplayAsset = type.GetProperty("displayAsset").GetBoolean();

            assets.Add(asset);
        }

        return assets;
    }
}