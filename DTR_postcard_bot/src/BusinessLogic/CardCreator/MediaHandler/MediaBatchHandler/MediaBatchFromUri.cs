using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;

public class MediaBatchFromUri(AssetOperator assetOperator) : IMediaBatchHandler
{
    public async Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAllAssets();

        var inputMediaPhotos = AssembleBatch(allRequiredAssets, assetType);

        return inputMediaPhotos;
    }
    
    public IEnumerable<InputMediaPhoto> AssembleBatch(List<Asset> assets, AssetType assetType)
    {
        var filteredAssets = assets.Where(a => a.Type == assetType);

        return filteredAssets
            .Select(asset => new InputMediaPhoto(media: InputFile.FromUri(asset.FileUrl)));
    }
}