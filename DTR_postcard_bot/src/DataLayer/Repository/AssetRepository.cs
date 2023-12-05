using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class AssetRepository(PostcardDbContext dbContext) : IRepository<Asset>
{
    public async Task<Asset> Get(long id)
    {
        return await dbContext.Assets.FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<IEnumerable<Asset>> GetAll(long id)
    {
        return dbContext.Assets.Where(a => a.Id == id);
    }

    public async Task Add(Asset id)
    {
        dbContext.Assets.Add(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Asset id)
    {
        dbContext.Assets.Update(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Asset id)
    {
        dbContext.Assets.Remove(id);
        await dbContext.SaveChangesAsync();
    }
}