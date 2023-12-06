using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;

namespace DTR_postcard_bot.DataLayer;

public class CardOperator(IRepository<Card> repository, ILogger<CardOperator> logger)
{
    public async Task<Card> FetchCard(long userId)
    {
        try
        {
            var card = await GetCard(userId);
            return card;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<Card> GetCard(long userId)
    {
        var card = await repository.Get(userId);
        return card;
    }
    
    public async Task RegisterNewCard(long userId, int lastMessageId)
    {
        try
        {
            Card card = new()
            {
                UserId = userId,
                LastBotMessageId = lastMessageId,
                CardCreationInProcess = true,
                Step = 0
            };
            
            await repository.Add(card);
        }
        catch (Exception e)
        {
            logger.LogError("Error creating new entry in db. ChatId: {ChatId}, error {Exception}", userId, e);
            throw;
        }
    }

    public async Task<bool> CheckIfExist(long userId)
    {
        try
        {
            var cardInfo = await GetCard(userId);
            if (cardInfo is null)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Error getting entry from db. ChatId: {ChatId}, error {Exception}", userId, e);
            throw;
        }
    }

    public async Task RemoveCard(Card card)
    {
        try
        {
            await repository.Remove(card);
        }
        catch (Exception e)
        {
            logger.LogError("Error removing entry from db. ChatId: {ChatId}, error {Exception}", card.UserId, e);
            throw;
        }
    }

    public async Task UpdateCard(Card card)
    {
        try
        {
            await repository.Update(card);
        }
        catch (Exception e)
        {
            logger.LogError("Error updating entry in db. ChatId: {ChatId}, error {Exception}", card.UserId, e);
            throw;
        }
    }
}