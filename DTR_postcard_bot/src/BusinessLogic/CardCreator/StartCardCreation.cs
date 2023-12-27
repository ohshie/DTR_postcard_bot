using DTR_postcard_bot.DataLayer;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger,
    CardOperator cardOperator, AssetTypeOperator assetTypeOperator,
    RequestMedia requestMedia)
{
    public async Task Handle(CallbackQuery query)
    {
        logger.LogInformation("User initiated Card creation UserId {UserId}", query.From.Id);

        if(await cardOperator.CheckIfExist(query.From.Id)) return;

        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        await cardOperator.RegisterNewCard(query.From.Id, query.Message.MessageId, assetTypes);
        
        await requestMedia.Execute(query: query);
    }
}