using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository.Interfaces;

public interface IAssetTypeRepository : IRepository<AssetType>
{
    Task BatchAdd(IEnumerable<AssetType> assetTypes);
    Task BatchRemove(IEnumerable<AssetType> entities);
}