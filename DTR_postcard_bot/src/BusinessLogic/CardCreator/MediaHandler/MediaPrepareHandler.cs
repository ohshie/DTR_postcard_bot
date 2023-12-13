using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using File = System.IO.File;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;

public class MediaPrepareHandler(AssetOperator assetOperator)
{
    public async Task<List<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await assetOperator.GetAllAssets();
        var filteredAssets = allRequiredAssets.Where(a => a.Type == assetType);

        List<InputMediaPhoto> inputMediaPhotos = new();
        foreach (var asset in filteredAssets)
        {
            InputMediaPhoto photo = new(media: InputFile.FromUri(asset.FileUrl));
            inputMediaPhotos.Add(photo);
        }

        return inputMediaPhotos;
    }
}