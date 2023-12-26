using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;

public interface IMediaBatchHandler
{
    Task<IEnumerable<InputMediaPhoto>> PrepareBatch(AssetType assetType);
}