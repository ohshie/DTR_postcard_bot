using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class CardRepository(PostcardDbContext dbContext) : IRepository<Card>
{
    public async Task<Card> Get(long id)
    {
        return await dbContext.Cards
            .FirstOrDefaultAsync(c => c.UserId == id);
    }

    public async Task<IQueryable<Card>> GetAll()
    {
        return dbContext.Cards
            .AsQueryable();
    }

    public async Task<IEnumerable<Card>> GetAll(int id)
    {
        return dbContext.Cards.Where(c => c.UserId == id).ToList();
    }

    public async Task Add(Card id)
    {
        dbContext.Cards.Add(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchAdd(IEnumerable<Card> entities)
    {
        await dbContext.Cards.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Card id)
    {
        dbContext.Cards.Update(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchUpdate(IEnumerable<Card> entities)
    {
       dbContext.Cards.UpdateRange(entities);
       await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Card id)
    {
        dbContext.Cards.Remove(id);
        await dbContext.SaveChangesAsync();
    }

    public async Task BatchRemove(IEnumerable<Card> entities)
    {
        dbContext.Cards.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}