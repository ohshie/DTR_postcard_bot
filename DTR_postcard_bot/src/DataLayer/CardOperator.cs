using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using DTR_postcard_bot.DataLayer.Repository.Interfaces;

namespace DTR_postcard_bot.DataLayer;

public class CardOperator(ICardRepository repository, ILogger<CardOperator> logger)
{
    public async Task<Card?> GetCard(long userId)
    {
        var card = await repository.Get(userId);
        
        if (card is not null) logger.LogInformation("Fetched {UserId} Card at step {CardStep}", card.UserId, card.Step);
        else logger.LogInformation("Failed to fetch card for {UserId}", userId);
        
        return card;
    }
    
    public async Task<Card> RegisterNewCard(long userId, int lastMessageId, IEnumerable<AssetType> assetTypes)
    {
        try
        {
            logger.LogInformation("Registering new card for {UserId} incomplete Card", userId);
            
            Card card = new()
            {
                UserId = userId,
                BotMessagesList = new(lastMessageId),
                CreationSteps = new(),
                CardCreationInProcess = true,
                Step = 0,
                AssetTypeIds = assetTypes.Select(at => at.Id).ToList()
            };
            
            await repository.Add(card);
            return card;
        }
        catch (Exception e)
        {
            logger.LogError("Error creating new entry in db. ChatId: {ChatId}, error {Exception}", userId, e.Message);
            throw;
        }
    }
    
    public async Task RemoveCard(Card card)
    {
        logger.LogInformation("Removing user {UserId} card entry", card.UserId);
        await repository.Remove(card);
    }

    public async Task UpdateCard(Card card)
    {
        logger.LogInformation("Updating {UserId} card entry", card.UserId);
        await repository.Update(card);
    }

    public async Task RemoveAllCards()
    {
        var cards = await repository.GetAll();
        await repository.BatchRemove(cards);
    }
}