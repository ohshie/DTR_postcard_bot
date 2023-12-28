using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.DataLayer;

public class AssetOperator(IRepository<Asset> repository, 
    ILogger<AssetOperator> logger)
{
    public async Task<Asset> Get(long id)
    {
        var requestedAsset = await repository.Get(id);
        return requestedAsset;
    }
    
    public async Task<IEnumerable<Asset>> GetAssetsByType(string type)
    {
        logger.LogInformation("Retrieving assets sorted by {AssetType}", type);

        var assets = await repository.GetAll();

        if (!assets.Any()) return new List<Asset>();
        
        return assets.Where(a => a.Type!.Type == type);
    }

    public async Task AddBatchAssets(List<Asset> assets)
    {
        await repository.BatchAdd(assets);
    }

    public async Task DeleteAllAssets()
    {
        logger.LogWarning("Deleting all registered assets");
        var assets = await repository.GetAll();
        await repository.BatchRemove(assets);
    }

    public async Task WriteTelegramFileIds(IEnumerable<string> fileIds, string assetType)
    {
        logger.LogInformation("Adding telegram Id's to assets of type: {AssetType}",assetType);
        
        var assetsSortedByTypes = (await repository.GetAll())
            .Where(a => a.Type!.Type == assetType && a.DisplayAsset)
            .ToArray();

        if (!string.IsNullOrEmpty(assetsSortedByTypes
                .Select(a => a.TelegramFileId)
                .FirstOrDefault())) return;

        var fileIdArray = fileIds.ToArray();
        
        if (assetsSortedByTypes.Length != fileIdArray.Length) return;

        var indexer = 0;
        
        foreach (var fileId in fileIdArray)
        {
            assetsSortedByTypes[indexer].TelegramFileId = fileId;
            indexer++;
        }

        await repository.BatchUpdate(assetsSortedByTypes);
    }
}