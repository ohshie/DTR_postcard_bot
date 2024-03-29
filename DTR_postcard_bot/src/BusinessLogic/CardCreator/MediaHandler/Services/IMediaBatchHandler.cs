using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;

public interface IMediaBatchHandler
{
    Task<(bool,IEnumerable<InputMediaPhoto>)> PrepareBatch(AssetType assetType);
}