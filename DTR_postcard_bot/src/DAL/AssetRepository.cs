using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.Repository;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL;

public class AssetRepository(PostcardDbContext dbContext, ILogger<AssetRepository> logger) 
    : GenericRepository<Asset>(dbContext, logger), IAssetRepository
{
    public async Task<IEnumerable<Asset?>> GetByType(string requestedType)
    {
        try
        {
            var allAssets = await GetAll();
            if (allAssets is null) return new List<Asset>();
           
            return allAssets.Where(a => a.Type.Type == requestedType).ToList();
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to fetch asset related to type {RequestedType} in {Type}", e.InnerException, requestedType, typeof(Asset));
            return new List<Asset?>();
        }
    }

    public async Task<IEnumerable<Asset>?> WriteTelegramFileIds(string[] fileIds, string assetType)
    {
        try
        {
            var allAssets = await GetAll();
            if (allAssets is null) return new List<Asset>();
        
            var assetsSortedByTypes = allAssets
                .Where(a => a.Type.Type == assetType && a.DisplayAsset)
                .ToList();
            
            for (var i = 0; i < fileIds.Length; i++)
            {
                assetsSortedByTypes[i].TelegramFileId = fileIds[i];
            }

            return assetsSortedByTypes;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform telegram ID matching with {Type}", e.InnerException, typeof(Asset));
            return null;
        }
    }

    public async Task<bool> BatchAdd(IEnumerable<Asset> assets)
    {
        try
        {
            await DbSet.AddRangeAsync(assets);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch add {Type}", e.InnerException, typeof(Asset));
            return false;
        }
    }

    public async Task<bool> BatchUpdate(IEnumerable<Asset> entities)
    {
        try
        {
            var incomingAssets = entities.ToList();
            
            var allAssets = await GetAll();
            if (allAssets is null) return false;

            var matchesAssets = allAssets.Where(incomingAssets.Contains).ToList();
            
            if (matchesAssets.Count == 0)
            {
                return await BatchAdd(incomingAssets);
            }
            
            DbSet.UpdateRange(matchesAssets);
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch update {Type}", e.InnerException, typeof(Asset));
            return false;
        }
    }

    public async Task<bool> RemoveAll()
    {
        try
        {
            var allAssets = await GetAll();
            if (allAssets is null) return false;
         
            await BatchRemove(allAssets);
            
            await dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence " +
                                                        "SET seq = 0 " +
                                                        "WHERE name = 'Assets'");
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to removing all {Type}", e.InnerException, typeof(Asset));
            return false;
        }
    }

    public async Task<bool> BatchRemove(IEnumerable<Asset?> entities, IEnumerable<Asset>? allAssets = null)
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
            logger.LogError("{Exception} while trying to perform batch remove {Type}", e.InnerException, typeof(Asset));
            return false;
        }
    }
}