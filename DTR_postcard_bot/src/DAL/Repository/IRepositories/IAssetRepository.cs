using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface IAssetRepository : IRepository<Asset>
{
    Task<IEnumerable<Asset>> GetByType(string requestedType);
    Task<IEnumerable<Asset>?> WriteTelegramFileIds(string[] fileIds, string assetType);
    Task<bool> BatchAdd(IEnumerable<Asset> assets);
    Task<bool> BatchUpdate(IEnumerable<Asset> entities);
    Task<bool> RemoveAll();
    Task<bool> BatchRemove(IEnumerable<Asset> entities, IEnumerable<Asset>? allAssets);
}