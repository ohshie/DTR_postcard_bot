using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;

public class MediaBatchFromStream : IMediaBatchHandler
{
    public Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<InputMediaPhoto> AssembleBatch(List<Asset> assets, AssetType assetType)
    {
        throw new NotImplementedException();
    }
}