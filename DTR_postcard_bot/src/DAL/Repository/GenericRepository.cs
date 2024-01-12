using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL.Repository;

public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly PostcardDbContext _dbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger<GenericRepository<TEntity>> Logger;

    protected GenericRepository(PostcardDbContext dbContext, 
        ILogger<GenericRepository<TEntity>> logger)
    {
        _dbContext = dbContext;
        Logger = logger;
        DbSet = dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> Get(long id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<bool> Add(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> Update(TEntity entity)
    {
        var entry = DbSet.Entry(entity);
        entry.State = EntityState.Modified;
        return true;
    }
    
    public virtual async Task Remove(object id)
    {
        var entity = await DbSet.FindAsync(id);
        await Remove(entity);
    }

    public virtual async Task<bool> Remove(TEntity entity)
    {
        if (_dbContext.Entry(entity).State is EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);
        
        return true;
    }
}