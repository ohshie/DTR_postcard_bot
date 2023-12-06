using DTR_postcard_bot.ChannelBase;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.AssetManager;

public class AssetLoader(AssetOperator assetOperator, 
    IConfiguration configuration)
{
    public async Task Load()
    {
        var currentAssets = await assetOperator.GetAllAssets();

        if (!currentAssets.Any()) await InitializeAssets();
    }

    private async Task InitializeAssets()
    {
        List<Asset> assets = new();
        
        foreach (var section in configuration.GetSection("AssetPaths").GetChildren())
        {
            var asset = new Asset()
            {
                Channel = configuration.GetSection("ChannelTag").Value!,
                Type = section.Value!,
                FilePath = PathBuilder(section.Value!),
            };
            
            assets.Add(asset);
        }

        await assetOperator.AddBatchAssets(assets);
    }

    private string PathBuilder(string assetType)
    {
        string[] filePaths =
        {
            AppDomain.CurrentDomain.BaseDirectory,
            assetType
        };

        return Path.Combine(filePaths);
    }
}