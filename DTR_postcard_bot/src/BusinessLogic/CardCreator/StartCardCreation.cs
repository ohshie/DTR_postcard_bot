using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger,
    CardOperator cardOperator,
    AssetTypeOperator assetTypeOperator,
    StatOperator statOperator,
    BotMessenger botMessenger)
{
    public async Task<Card> Handle(CallbackQuery query, Card? card = null)
    {
        if (card is not null)
        { 
            logger.LogInformation("Removing traces of an old card {UserId}", query.From.Id);
            
            await botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);
            await cardOperator.RemoveCard(card);
        }
        
        logger.LogInformation("Registering new Card creation for UserId {UserId}", query.From.Id);
        
        var assetTypes = await assetTypeOperator.GetAllAssetTypes();
        
        card = await cardOperator.RegisterNewCard(query.From.Id, query.Message.MessageId, assetTypes);

        await statOperator.RegisterUser(card.UserId, query.From.Username);
        await statOperator.IncrementStartedCard(card.UserId);

        return card;
    }
}