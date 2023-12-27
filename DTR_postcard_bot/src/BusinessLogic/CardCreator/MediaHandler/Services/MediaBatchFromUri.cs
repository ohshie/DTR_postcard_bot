using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public class MediaBatchFromUri(AssetOperator assetOperator) : IMediaBatchHandler
{
    public async Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);

        var inputMediaPhotos = AssembleBatch(allRequiredAssets);

        return inputMediaPhotos;
    }

    private IEnumerable<InputMediaPhoto> AssembleBatch(IEnumerable<Asset> assets)
    {
        return assets
            .Select(asset => new InputMediaPhoto(media: InputFile.FromUri(asset.FileUrl)));
    }
}