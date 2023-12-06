using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> Get(long id);
    Task<IEnumerable<TEntity>> GetAll(string id);
    Task Add(TEntity id);
    Task BatchAdd(List<TEntity> entities);
    Task Update(TEntity id);
    Task Remove(TEntity id);
}