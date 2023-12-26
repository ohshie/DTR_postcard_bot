using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public class MediaBatchFromStream(AssetOperator assetOperator) : IMediaBatchHandler
{
    public async Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);
        
        var inputMediaPhotos = AssembleBatch(allRequiredAssets.Where(a => a.DisplayAsset));

        return inputMediaPhotos;
    }

    private IEnumerable<InputMediaPhoto> AssembleBatch(IEnumerable<Asset> assets)
    {
        List<InputMediaPhoto> photos = new();
        
        foreach (var asset in assets)
        {
            var stream = new FileStream(asset.FileUrl, FileMode.Open); 
            InputMediaPhoto mediaPhoto = new(new InputFileStream(stream, 
                fileName: $"{asset.Type.Type}_{asset.FileName}"));
            
            photos.Add(mediaPhoto);
        }

        return photos;
    }
}