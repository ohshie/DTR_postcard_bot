using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class CardRepository(PostcardDbContext dbContext) : IRepository<Card>
{
    public async Task<Card> Get(long id)
    {
        return await dbContext.CardCreationTables.FirstOrDefaultAsync(c => c.UserId == id);
    }

    public async Task<IEnumerable<Card>> GetAll(string id)
    {
        return dbContext.CardCreationTables.Where(c => c.UserId.ToString() == id).ToList();
    }

    public async Task Add(Card id)
    {
        dbContext.CardCreationTables.Add(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchAdd(List<Card> entities)
    {
        await dbContext.CardCreationTables.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Card id)
    {
        dbContext.CardCreationTables.Update(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Card id)
    {
        dbContext.CardCreationTables.Remove(id);
        await dbContext.SaveChangesAsync();
    }
}