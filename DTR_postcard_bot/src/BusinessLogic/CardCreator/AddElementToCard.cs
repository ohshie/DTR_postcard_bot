using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class AddElementToCard(ILogger<AddElementToCard> logger, 
    IUnitOfWork unitOfWork, 
    StartCardCreation startCardCreation, 
    CompleteAndSendCard completeAndSendCard,
    RequestMedia requestMedia) : CardCreatorBase(logger, startCardCreation, unitOfWork)
{
    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        if (query is null) return;

        var mediaTypeAndIdArray = query.Data!.Split(" ").Reverse().Take(2).ToArray();
        
        var mediaTypeAndId = $"{mediaTypeAndIdArray.Last()} {mediaTypeAndIdArray.First()}";
        
        card.CreationSteps.Add(mediaTypeAndId);

        await UpdateCard(card);
        
        if (card.Step < card.AssetTypeIds.Count)
        {
            NextTask = requestMedia;
        }
        else
        {
            NextTask = completeAndSendCard;
        }
    }

    private async Task UpdateCard(Card card)
    {
        await unitOfWork.Cards.Update(card);
        await unitOfWork.CompleteAsync();
    }
}