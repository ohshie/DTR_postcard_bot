using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class StatRepository(PostcardDbContext dbContext) : IRepository<Stat>
{
    public async Task<Stat> Get(long id)
    {
        return await dbContext.Stats.FirstOrDefaultAsync(s => s.UserId == id);
    }

    public async Task<IQueryable<Stat>> GetAll()
    {
        return dbContext.Stats.AsQueryable();
    }

    public async Task Add(Stat id)
    {
        dbContext.Stats.Add(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchAdd(IEnumerable<Stat> entities)
    {
        await dbContext.Stats.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Stat entity)
    {
        dbContext.Stats.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchUpdate(IEnumerable<Stat> entities)
    {
        dbContext.Stats.UpdateRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Stat entity)
    {
        dbContext.Stats.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchRemove(IEnumerable<Stat> entities)
    {
        dbContext.Stats.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}