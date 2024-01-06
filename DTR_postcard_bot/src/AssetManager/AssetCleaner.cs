using DTR_postcard_bot.DataLayer;

namespace DTR_postcard_bot.AssetManager;

public class AssetCleaner(TextOperator textOperator, 
    AssetOperator assetOperator, 
    AssetTypeOperator assetTypeOperator,
    ILogger<AssetCleaner> logger)
{
    internal async Task Execute()
    {
        logger.LogWarning("Deleting old assets, texts and asset types");
        
        await textOperator.ClearCurrentTexts();
        await assetOperator.DeleteAllAssets();
        await assetTypeOperator.BatchDeleteAssetTypes();
    }
}