using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger, 
    BotMessenger botMessenger,
    IUnitOfWork unitOfWork)
{
    public async Task<Card?> Handle(CallbackQuery query)
    {
        if (query.Data == CallbackList.CreateNew)
        {
            logger.LogWarning("Leftover card found for {UserId}, will delete it now", query.From.Id);
            await OldCardCleanUp(query);
        }
        
        logger.LogInformation("Registering new Card creation for UserId {UserId}", query.From.Id);
        
        var assetTypes = await unitOfWork.AssetTypes.GetAll() ?? new List<AssetType>();
        
        var card = await RegisterNewCard(query, assetTypes);
        
        return card;
    }

    private async Task<Card> RegisterNewCard(CallbackQuery query, IEnumerable<AssetType> assetTypes)
    {
        var userId = query.From.Id;
        var userName = query.From.Username;
        var messageId = query.Message!.MessageId;
        
        Card card = new()
        {
            UserId = userId,
            BotMessagesList = new List<int>(messageId),
            CreationSteps = new List<string>(),
            CardCreationInProcess = true,
            Step = 0,
            AssetTypeIds = assetTypes.Select(at => at.Id).ToList()
        };

        await UpdateDb(card, userId, userName);
        
        return card;
    }

    private async Task UpdateDb(Card card, long userId, string? userName)
    {
        await unitOfWork.Cards.Add(card);
        await unitOfWork.Stats.UpdateOrAdd(userId, userName);
        
        await unitOfWork.CompleteAsync();
    }

    private async Task OldCardCleanUp(CallbackQuery query)
    {
        var card = await unitOfWork.Cards.Get(query.From.Id);

        if (card is not null)
        {
            await unitOfWork.Cards.Remove(card);
            await unitOfWork.CompleteAsync();
            await botMessenger.DeleteMessageAsync(query.From.Id, query.Message!.MessageId);
        }
    }
}