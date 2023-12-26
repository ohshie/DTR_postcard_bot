using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using Telegram.Bot.Types.Enums;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;

public class MediaBatchFromStream(AssetOperator assetOperator) : IMediaBatchHandler
{
    public async Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);
        
        var inputMediaPhotos = AssembleBatch(allRequiredAssets!);

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