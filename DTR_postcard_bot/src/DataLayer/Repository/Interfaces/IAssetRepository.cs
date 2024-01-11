using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository.Interfaces;

public interface IAssetRepository : IRepository<Asset>
{
    Task BatchAdd(IEnumerable<Asset> assets);
    Task BatchUpdate(IEnumerable<Asset> entities);
    Task BatchRemove(IEnumerable<Asset> entities);
}