namespace DTR_postcard_bot.DataLayer.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> Get(long id);
    Task<IQueryable<TEntity>> GetAll();
    Task Add(TEntity id);
    Task BatchAdd(IEnumerable<TEntity> entities);
    Task Update(TEntity id);
    Task BatchUpdate(IEnumerable<TEntity> entities);
    Task Remove(TEntity id);
    Task BatchRemove(IEnumerable<TEntity> entities);
}