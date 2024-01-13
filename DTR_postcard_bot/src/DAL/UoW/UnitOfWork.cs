using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.DAL.UoW;

public class UnitOfWork(PostcardDbContext dbContext, ILogger<UnitOfWork> logger,
    ICardRepository cards, IAssetRepository assets, IAssetTypeRepository assetTypes,
    IStatRepository stats, ITextRepository texts) : IUnitOfWork, IDisposable
{
    public ICardRepository Cards { get; private set; } = cards;
    public IAssetRepository Assets { get; private set; } = assets;
    public IAssetTypeRepository AssetTypes { get; private set; } = assetTypes;
    public IStatRepository Stats { get; private set; } = stats;
    public ITextRepository Texts { get; private set; } = texts;
    
    public async Task CompleteAsync()
    {
        logger.LogInformation("Saving changes to Db");
        await dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }
}