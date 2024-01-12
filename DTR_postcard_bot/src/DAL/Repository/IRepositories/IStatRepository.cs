using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface IStatRepository : IRepository<Stat>
{
    public Task<Stat?> Get(int id);
    public Task<bool> UpdateOrAdd(long id, string? name = null);
    public Task<bool> UpdateOnFinish(long id);
}