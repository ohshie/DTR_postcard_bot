using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class TextRepository(PostcardDbContext dbContext) : IRepository<Text>
{
    public async Task<Text> Get(long id)
    {
        return await dbContext.Texts.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IQueryable<Text>> GetAll()
    {
        return dbContext.Texts.AsQueryable()
            .AsNoTracking();
    }

    public async Task Add(Text text)
    {
        dbContext.Texts.Add(text);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchAdd(IEnumerable<Text> entities)
    {
        await dbContext.Texts.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Text text)
    {
        dbContext.Texts.Update(text);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchUpdate(IEnumerable<Text> entities)
    {
        dbContext.Texts.UpdateRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Text text)
    {
        dbContext.Texts.Remove(text);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchRemove(IEnumerable<Text> entities)
    {
        dbContext.Texts.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}