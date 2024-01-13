namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> Get(long id);
    Task<IEnumerable<TEntity>?> GetAll();
    Task<bool> Add(TEntity? entity);
    Task<bool> Update(TEntity? entity);
    Task<bool> Remove(TEntity? entity);
}