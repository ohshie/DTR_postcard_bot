using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetTypeRepository(PostcardDbContext dbContext) : IRepository<AssetType>
{
    public async Task<AssetType> Get(long id)
    {
        return await dbContext.AssetTypes.FirstOrDefaultAsync(at => at.Id == id);
    }

    public async Task<IQueryable<AssetType>> GetAll()
    {
        return dbContext.AssetTypes.AsQueryable();
    }

    public async Task<IEnumerable<AssetType>> GetAllByType(AssetType requestedType)
    {
        return await dbContext.AssetTypes.Where(at => at.Type == requestedType.Type).ToListAsync();
        
    }

    public async Task Add(AssetType entity)
    {
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchAdd(IEnumerable<AssetType> entities)
    {
        await dbContext.AssetTypes.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(AssetType entity)
    {
        dbContext.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchUpdate(IEnumerable<AssetType> entities)
    {
        dbContext.AssetTypes.UpdateRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(AssetType entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchRemove(IEnumerable<AssetType> entities)
    {
        dbContext.AssetTypes.RemoveRange(entities);
        await dbContext.SaveChangesAsync();

        await dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence " +
                                                    "SET seq = 0 " +
                                                    "WHERE name = 'AssetTypes'");
    }
}