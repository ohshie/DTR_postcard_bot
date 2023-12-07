using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.DataLayer;

public class AssetTypeOperator(IRepository<AssetType> repository)
{
    public async Task BatchCreateTypes(List<AssetType> assetTypes)
    {
        await repository.BatchAdd(assetTypes);
    }

    public async Task BatchDeleteAssetTypes()
    {
        var assetTypes = await repository.GetAll();
        await repository.BatchRemove(assetTypes);
    }

    public async Task<IEnumerable<AssetType>> GetAllAssetTypes()
    {
        return await repository.GetAll();
    }
}