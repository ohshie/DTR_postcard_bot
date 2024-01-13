using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.Repository;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL;

public class CardRepository(PostcardDbContext dbContext, ILogger<CardRepository> logger) : 
    GenericRepository<Card>(dbContext, logger), ICardRepository
{
    public override async Task<IEnumerable<Card>?> GetAll()
    {
        try
        {
            return await DbSet.ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to get all {Type}", e.InnerException, typeof(Card));
            return new List<Card>();
        }
    }

    public override async Task<bool> Update(Card? entity)
    {
        try
        {
            if (entity is null)
            {
                return await Add(entity);
            }
            
            var existingCard = await DbSet.FirstOrDefaultAsync(c => c != null && c.UserId == entity.UserId);

            existingCard = entity;
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to update {Type}", e.InnerException, typeof(Card));
            return false;
        }
    }

    public override async Task<bool> Remove(Card? entity)
    {
        try
        {
            if (entity is null) return false;
          
            var existingCard = await DbSet.FirstOrDefaultAsync(c => c != null && c.UserId == entity.UserId);
           
            DbSet.Remove(existingCard);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to delete {Type}", e.InnerException, typeof(Card));
            return false;
        }
    }
    
    public async Task<bool> BatchRemove(IEnumerable<Card> entities)
    {
        try
        {
            DbSet.RemoveRange(entities);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to delete all {Type}", e.InnerException, typeof(Card));
            return false;
        }
    }
}