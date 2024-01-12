using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public class MediaBatchFromUri(IUnitOfWork unitOfWork) : IMediaBatchHandler
{
    public async Task<(bool, IEnumerable<InputMediaPhoto>)> PrepareBatch(AssetType assetType)
    {
        var allRequiredAssets = await unitOfWork.Assets.GetByType(assetType.Type) as HashSet<Asset>;

        var inputMediaPhotos = AssembleBatch(allRequiredAssets!);

        var tgFileIdExist = string.IsNullOrEmpty(allRequiredAssets!.Select(a => a.TelegramFileId).FirstOrDefault());

        return (tgFileIdExist, inputMediaPhotos);
    }

    private IEnumerable<InputMediaPhoto> AssembleBatch(IEnumerable<Asset> assets)
    {
        return assets
            .Select(asset => new InputMediaPhoto(media: InputFile.FromUri(asset.FileUrl)));
    }
}