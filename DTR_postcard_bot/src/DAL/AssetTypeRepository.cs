using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.Repository;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL;

public class AssetTypeRepository(PostcardDbContext dbContext, ILogger<AssetTypeRepository> logger) : GenericRepository<AssetType>(dbContext, logger), IAssetTypeRepository
{
    public override async Task<bool> Update(AssetType? entity)
    {
        try
        {
            if (entity is null) return false;
         
            var existingType = await DbSet.FirstOrDefaultAsync(at => at != null && at.Id == entity.Id);
            if (existingType is null) return await Add(entity);

            existingType = entity;
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to update {Type}", e.InnerException, typeof(AssetType));
            return false;
        }
    }

    public async Task<AssetType?> Get(int id)
    {
        try
        {
            return await DbSet.FindAsync(id);
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to fetch {Type} with {Id}",
                e.InnerException, typeof(AssetType), id);
            throw;
        }
    }

    public async Task<bool> BatchAdd(IEnumerable<AssetType> entities)
    {
        try
        {
            await DbSet.AddRangeAsync(entities);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch add {Type}", e.InnerException, typeof(AssetType));
            return false;
        }
    }

    public async Task<bool> BatchUpdate(IEnumerable<AssetType> entities)
    {
        try
        {
            var incomingAssetsTypes = entities.ToList();
            
            var allAssetsTypes = await GetAll() ?? new List<AssetType>();
          
            var matchesAssetsTypes = allAssetsTypes.Where(incomingAssetsTypes.Contains).ToList();
            
            if (!matchesAssetsTypes.Any())
            {
                return await BatchAdd(incomingAssetsTypes);
            }
            
            DbSet.UpdateRange(matchesAssetsTypes);
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch update {Type}", e.InnerException, typeof(AssetType));
            return false;
        }
    }

    public override async Task<bool> Remove(AssetType? entity)
    {
        try
        {
            if (entity is null) return false;
            
            var existingAssetType = await DbSet.FirstOrDefaultAsync(at => at != null && at.Id == entity.Id);
            if (existingAssetType is null) return false;
           
            DbSet.Remove(existingAssetType);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to delete {Type}", e.InnerException, typeof(Card));
            return false;
        }
    }

    public async Task<bool> RemoveAll()
    {
        try
        {
            var allAssets = await GetAll() ?? new List<AssetType>();
            
            await BatchRemove(allAssets);
            
            await dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence " +
                                                        "SET seq = 0 " +
                                                        "WHERE name = 'AssetTypes'");

            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to removing all {Type}", e.InnerException, typeof(Asset));
            return false;
        }
    }

    public async Task<bool> BatchRemove(IEnumerable<AssetType?> entities, IEnumerable<AssetType>? allAssets = null)
    {
        try
        {
            if (allAssets is not null)
            {
                entities = allAssets.Where(entities.Contains).ToList();
            }

            DbSet.RemoveRange(entities!);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch remove {Type}", e.InnerException, typeof(AssetType));
            return false;
        }
    }
}