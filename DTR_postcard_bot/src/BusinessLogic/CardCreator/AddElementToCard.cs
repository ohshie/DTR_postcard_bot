using DTR_postcard_bot.BusinessLogic.CardCreator.ElementsHandler;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class AddElementToCard : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly RequestMedia _requestMedia;

    public AddElementToCard(ILogger<AddElementToCard> logger, 
        CardOperator cardOperator , RequestMedia requestMedia) : 
        base(logger, cardOperator)
    {
        _cardOperator = cardOperator;
        _requestMedia = requestMedia;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        if (query is null) return;

        var mediaTypeAndIdEnumerable = query.Data.Split(" ").Reverse().Take(2);
        var mediaTypeAndId = $"{mediaTypeAndIdEnumerable.Last()} {mediaTypeAndIdEnumerable.First()}";
        
        card.CreationSteps.Add(mediaTypeAndId);

        await _cardOperator.UpdateCard(card);

        var test = card.CreationSteps;
        
        NextTask = _requestMedia;
    }
}