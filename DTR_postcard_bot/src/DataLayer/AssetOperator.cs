using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetOperator(IRepository<Asset> repository, 
    ILogger<AssetOperator> logger,
    IConfiguration configuration)
{
    public async Task<Asset> Get(long id)
    {
        var requestedAsset = await repository.Get(id);
        return requestedAsset;
    }
    
    public async Task<List<Asset>> GetAllAssets()
    {
        var assetList = await repository.GetAll() as List<Asset>;

        if (assetList is not null && assetList.Any())
        {
            var test = assetList.Where(a => a.Channel == configuration.GetSection("ChannelTag").Value);
            return assetList;
        }
        
        assetList = new();
        return assetList;

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
}