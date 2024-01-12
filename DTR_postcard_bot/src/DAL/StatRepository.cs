using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.Repository;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL;

public class StatRepository(PostcardDbContext dbContext, 
    ILogger<StatRepository> logger) : GenericRepository<Stat>(dbContext, logger), IStatRepository
{
    public async Task<Stat?> Get(int id)
    {
        try
        {
            return await DbSet.FindAsync(id);
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to fetch {Type} with {Id}",
                e.InnerException, typeof(Stat), id);
            throw;
        }
    }

    public async Task<bool> UpdateOrAdd(long userId, string? name = null)
    {
        try
        {
            var currentStat = await DbSet.FirstOrDefaultAsync(s => s.UserId == userId);

            if (currentStat is not null)
            {
                return await Update(currentStat);
            }
            
            return await base.Add(new Stat()
            {
                UserId = userId,
                CreatedCards = 1,
                UserName = string.IsNullOrEmpty(name) ? "Anonymous" : name
            });
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to create new {Type}", e.InnerException, typeof(Stat));
            return false;
        }
    }

    public override async Task<IEnumerable<Stat>> GetAll()
    {
        try
        {
            return await DbSet.ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to get all {Type}", e.InnerException, typeof(Stat));
            return new List<Stat>();
        }
    }
    
    public override async Task<bool> Update(Stat entity)
    {
        try
        {
            var existingStat = await DbSet.FirstOrDefaultAsync(c => c.UserId == entity.UserId);
            if (existingStat is null) return await Add(entity);

            existingStat.CreatedCards++;
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to update {Type}", e.InnerException, typeof(Stat));
            return false;
        }
    }
    
    public override async Task<bool> Remove(Stat entity)
    {
        try
        {
            var existingStat = await DbSet.FirstOrDefaultAsync(c => c.UserId == entity.UserId);
            if (existingStat is null) return false;
           
            DbSet.Remove(existingStat);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to delete {Type}", e.InnerException, typeof(Stat));
            return false;
        }
    }

    public async Task<bool> UpdateOnFinish(long id)
    {
        try
        {
            var existingStat = await DbSet.FirstOrDefaultAsync(c => c.UserId == id);
            if (existingStat is null) return await Add(new Stat()
            {
                UserId = id,
                UserName = "Anonymous"
            });

            existingStat.CreatedCards++;
            existingStat.DroppedCards--;
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to update {Type}", e.InnerException, typeof(Stat));
            return false;
        }
    }
}