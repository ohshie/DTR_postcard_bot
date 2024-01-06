using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger,
    CardOperator cardOperator,
    AssetTypeOperator assetTypeOperator,
    StatOperator statOperator, BotMessenger botMessenger)
{
    public async Task<Card> Handle(CallbackQuery query)
    {
        if (query.Data == CallbackList.CreateNew)
        {
            logger.LogWarning("Leftover card found for {UserId}, will delete it now", query.From.Id);
            await OldCardCleanUp(query);
        }
        
        logger.LogInformation("Registering new Card creation for UserId {UserId}", query.From.Id);
        
        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        
        var card = await cardOperator.RegisterNewCard(query.From.Id, query.Message!.MessageId, assetTypes);

        await statOperator.RegisterUser(card.UserId, query.From.Username);
        await statOperator.IncrementStartedCard(card.UserId);

        return card;
    }

    private async Task OldCardCleanUp(CallbackQuery query)
    {
        var card = await cardOperator.GetCard(query.From.Id);

        if (card is not null)
        {
            await cardOperator.RemoveCard(card);
            await botMessenger.DeleteMessageAsync(query.From.Id, query.Message!.MessageId);
        }
    }
}