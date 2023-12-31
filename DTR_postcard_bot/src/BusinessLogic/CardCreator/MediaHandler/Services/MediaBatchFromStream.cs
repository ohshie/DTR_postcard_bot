using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public class MediaBatchFromStream(AssetOperator assetOperator) : IMediaBatchHandler
{
    public async Task<(bool, IEnumerable<InputMediaPhoto>)> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAssetsByType(assetType.Type);
        
        var requiredAssets = allRequiredAssets as Asset[] ?? allRequiredAssets.ToArray();
        
        var inputMediaPhotos = AssembleBatch(requiredAssets!.Where(a => a.DisplayAsset));

        var tgFileIdExist = !string.IsNullOrEmpty(requiredAssets!
            .Select(a => a.TelegramFileId)
            .FirstOrDefault());
        
        return  (tgFileIdExist, inputMediaPhotos);
    }

    private IEnumerable<InputMediaPhoto> AssembleBatch(IEnumerable<Asset> assets)
    {
        List<InputMediaPhoto> photos = new();
        
        foreach (var asset in assets)
        {
            if (!string.IsNullOrEmpty(asset.TelegramFileId))
            {
               photos.Add(new InputMediaPhoto(InputFile.FromFileId(asset.TelegramFileId))); 
               continue;
            }
            
            var stream = new FileStream(asset.FileUrl, FileMode.Open); 
            InputMediaPhoto mediaPhoto = new(new InputFileStream(stream, 
                fileName: $"{asset.Type.Type}_{asset.FileName}"));
            
            photos.Add(mediaPhoto);
        }

        return photos;
    }
}