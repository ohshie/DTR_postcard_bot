using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetOperator(IRepository<Asset> repository, 
    ILogger<AssetOperator> logger,
    IConfiguration configuration)
{
    public async Task<List<Asset>> GetAllAssets()
    {
        var assetList = await repository.GetAll(id: configuration.GetSection("ChannelTag").Value) as List<Asset>;

        if (assetList is not null && assetList.Any()) return assetList;
        
        assetList = new();
        return assetList;

    }

    public async Task AddBatchAssets(List<Asset> assets)
    {
        await repository.BatchAdd(assets);
    }
}