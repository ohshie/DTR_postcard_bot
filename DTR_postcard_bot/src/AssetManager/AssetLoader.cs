using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;

namespace DTR_postcard_bot.AssetManager;

public class AssetLoader(AssetOperator assetOperator, 
    IConfiguration configuration)
{
    public async Task Load()
    {
        var currentAssetsInDb = await assetOperator.GetAllAssets();
        var assetsInFolders = await InitializeAssets();

        if (!currentAssetsInDb.Any())
        {
            await assetOperator.AddBatchAssets(assetsInFolders);
        }
        else
        {
            List<Asset> assetsToBeAdded = CompareAssets(currentAssetsInDb, assetsInFolders);
            await assetOperator.AddBatchAssets(assetsToBeAdded);
        }
    }

    private async Task<List<Asset>> InitializeAssets()
    {
        List<Asset> assets = new();
        
        foreach (var section in configuration.GetSection("AssetPaths").GetChildren())
        {
            var typePath = PathBuilder(section.Value!);
            var filesPaths = Directory.EnumerateFiles(typePath).ToArray();
            foreach (var file in filesPaths)
            {
                var asset = new Asset()
                {
                    Channel = configuration.GetSection("ChannelTag").Value!,
                    Type = section.Value!,
                    FilePath = file
                };
            
                assets.Add(asset);
            }
        }

        return assets;
    }

    private List<Asset> CompareAssets(List<Asset> assetsInDb, List<Asset> assetsInFolders)
    {
        List<Asset> assetsToBeAdded = assetsInFolders.Where(currentAsset => !assetsInDb
                .Any(a => a.FilePath == currentAsset.FilePath
                && a.Channel == currentAsset.Channel
                && a.Type == currentAsset.Type))
            .ToList();

        return assetsToBeAdded;
    }

    private string PathBuilder(string assetType)
    {
        string[] filePaths =
        {
            AppDomain.CurrentDomain.BaseDirectory,
            "assets",
            assetType,
        };

        return Path.Combine(filePaths);
    }
}