using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger,
    CardOperator cardOperator,
    AssetTypeOperator assetTypeOperator,
    StatOperator statOperator,
    RequestMedia requestMedia)
{
    public async Task Handle(CallbackQuery query)
    {
        logger.LogInformation("User initiated Card creation UserId {UserId}", query.From.Id);

        if(await cardOperator.CheckIfExist(query.From.Id)) return;

        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        
        await cardOperator.RegisterNewCard(query.From.Id, query.Message.MessageId, assetTypes);

        await statOperator.RegisterUser(query.From.Id, query.From.Username);
        await statOperator.IncrementStartedCard(query.From.Id);
        
        await requestMedia.Execute(query: query);
    }
}