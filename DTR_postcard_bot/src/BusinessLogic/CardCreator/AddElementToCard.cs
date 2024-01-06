using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class AddElementToCard : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly RequestMedia _requestMedia;
    private readonly CompleteAndSendCard _completeAndSendCard;

    public AddElementToCard(ILogger<AddElementToCard> logger, 
        CardOperator cardOperator , RequestMedia requestMedia, CompleteAndSendCard completeAndSendCard,
        StartCardCreation startCardCreation) : 
        base(logger, cardOperator, startCardCreation)
    {
        _cardOperator = cardOperator;
        _requestMedia = requestMedia;
        _completeAndSendCard = completeAndSendCard;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        if (query is null) return;

        var mediaTypeAndIdArray = query.Data!.Split(" ").Reverse().Take(2).ToArray();
        
        var mediaTypeAndId = $"{mediaTypeAndIdArray.Last()} {mediaTypeAndIdArray.First()}";
        
        card.CreationSteps.Add(mediaTypeAndId);

        await _cardOperator.UpdateCard(card);
        
        if (card.Step < card.AssetTypeIds.Count)
        {
            NextTask = _requestMedia;
        }
        else
        {
            NextTask = _completeAndSendCard;
        }
    }
}