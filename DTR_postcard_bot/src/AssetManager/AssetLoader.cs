using System.Text.Json;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;

namespace DTR_postcard_bot.AssetManager;

public class AssetLoader(AssetOperator assetOperator, AssetTypeOperator assetTypeOperator,
    IConfiguration configuration, ILogger<AssetLoader> logger)
{
    public async Task Load()
    {
        await DumpOldAssets();

        var assetJson = await LoadJson();
        
        await InitializeAssetTypes(assetJson);
        await InitializeAssets(assetJson);
    }

    private async Task InitializeAssetTypes(JsonDocument jDoc)
    {
        List<AssetType> assetTypes = new();
        
        var jsonChannels = jDoc.RootElement.GetProperty("assetTypesByChannel").EnumerateObject();

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

        await assetTypeOperator.BatchCreateTypes(assetTypes);
    }

    private async Task InitializeAssets(JsonDocument jDoc)
    {
        List<Asset> assets = new();
        
        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        
        foreach (var type in jDoc.RootElement.GetProperty("mediaLinks").EnumerateArray())
        {
            var asset = new Asset()
            {
                Channel = type.GetProperty("channel").ToString(),
                Type = assetTypes.FirstOrDefault(at => at.Type == type.GetProperty("type").ToString()),
                FileName = type.GetProperty("filePath").ToString()
            };

            type.TryGetProperty("text", out var text);
            asset.Text = text.ToString();

            asset.FileUrl = string.IsNullOrEmpty(type.GetProperty("fileUrl").ToString()) 
                ? Helpers.PathBuilder("assets", asset.Type.Type, asset.FileName) 
                : type.GetProperty("fileUrl").ToString();

            asset.OutputAsset = type.GetProperty("outputAsset").GetBoolean();
            asset.DisplayAsset = type.GetProperty("displayAsset").GetBoolean();
            
            assets.Add(asset);
        }

        await assetOperator.AddBatchAssets(assets);
    }

    private async Task<JsonDocument> LoadJson()
    {
        var assetJson = await File.ReadAllTextAsync(configuration.GetSection("PathToAssetJson").Value);
        JsonDocument jDoc = JsonDocument.Parse(assetJson);
        
        return jDoc;
    }

    private async Task DumpOldAssets()
    {
        logger.LogWarning("Deleting old assets and asset types");
        
        await assetOperator.DeleteAllAssets();
        await assetTypeOperator.BatchDeleteAssetTypes();
    }
}