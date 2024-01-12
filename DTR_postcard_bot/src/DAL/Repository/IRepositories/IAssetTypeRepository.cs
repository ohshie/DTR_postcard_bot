using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface IAssetTypeRepository : IRepository<AssetType>
{
    Task<AssetType?> Get(int id);
    Task<bool> BatchAdd(IEnumerable<AssetType> assetTypes);
    Task<bool> RemoveAll();
    Task<bool> BatchRemove(IEnumerable<AssetType> entities, IEnumerable<AssetType>? allAssets = null);
}