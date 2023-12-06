using DTR_postcard_bot.ChannelBase;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetOperator(IRepository<Asset> repository, ChannelMapper channelMapper)
{
    public async Task<Asset[]> GetAllAssets()
    {
        var assetList = await repository.GetAll(id: channelMapper.ChannelId) as Asset[];

        if (assetList is null || !assetList.Any())
        {
            assetList = Array.Empty<Asset>();
            return assetList;
        }
        
        return assetList;
    }

    public async Task AddBatchAssets(List<Asset> assets)
    {
        await repository.BatchAdd(assets);
    }
}