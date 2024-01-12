using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.Repository;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL;

public class TextRepository(PostcardDbContext dbContext, ILogger<TextRepository> logger) :  GenericRepository<Text>(dbContext, logger),ITextRepository
{
    public async Task<string> GetByTrigger(string trigger)
    {
        try
        {
            var text = await DbSet.FirstOrDefaultAsync(t => t.Trigger == trigger);

            return text is not null ? text.Content : string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to create new {Type}", 
                e.InnerException, typeof(Text));
            return string.Empty;
        }
    }

    public async Task<bool> BatchAdd(IEnumerable<Text> texts)
    {
        try
        {
            await DbSet.AddRangeAsync(texts);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch add {Type}",
                e.InnerException, typeof(Text));
            return false;
        }
    }

    public async Task<bool> RemoveAll()
    {
        try
        {
            var allTexts = await GetAll();
            await BatchRemove(allTexts);
            
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

    public async Task<bool> BatchRemove(IEnumerable<Text> texts, IEnumerable<Text>? allTexts = null)
    {
        try
        {
            if (allTexts is not null)
            {
                texts = allTexts.Where(texts.Contains).ToList();
            }

            DbSet.RemoveRange(texts);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError("{Exception} while trying to perform batch remove {Type}",
                e.InnerException, typeof(Text));
            return false;
        }
    }
}