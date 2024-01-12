using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.AssetManager;

public class AssetCleaner(IUnitOfWork unitOfWork,
    ILogger<AssetCleaner> logger)
{
    internal async Task Execute()
    {
        logger.LogWarning("Deleting old assets, texts and asset types");
        
        await unitOfWork.Texts.RemoveAll();
        await unitOfWork.Assets.RemoveAll();
        await unitOfWork.AssetTypes.RemoveAll();

        await unitOfWork.CompleteAsync();
    }
}