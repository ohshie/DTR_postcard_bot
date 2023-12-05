using DTR_postcard_bot.ChannelBase;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetOperator(IRepository<Asset> repository, ChannelMapper channelMapper)
{
    public async Task<Asset[]> GetAllAssets()
    {
        var assetList = await repository.GetAll(id: channelMapper.ChannelId) as Asset[];
        
        if (assetList.Any()) return assetList;
        
        assetList = Array.Empty<Asset>();
        return assetList;

    }
}