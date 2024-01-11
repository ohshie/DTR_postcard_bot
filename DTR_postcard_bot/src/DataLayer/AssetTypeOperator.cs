using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using DTR_postcard_bot.DataLayer.Repository.Interfaces;

namespace DTR_postcard_bot.DataLayer;

public class AssetTypeOperator(IAssetTypeRepository repository)
{
    public async Task<AssetType> GetById(int id)
    {
        return await repository.Get(id);
    }
    
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