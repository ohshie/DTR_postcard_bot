using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public abstract class CardCreatorBase
{
    private readonly ILogger<CardCreatorBase> _logger;
    private readonly CardOperator _cardOperator;
    private readonly StartCardCreation _startCardCreation;

    protected CardCreatorBase? NextTask;

    protected CardCreatorBase( 
        ILogger<CardCreatorBase> logger, 
        CardOperator cardOperator,
        StartCardCreation startCardCreation)
    {
        _logger = logger;
        _cardOperator = cardOperator;
        _startCardCreation = startCardCreation;
    }
    
    public async Task Execute(CallbackQuery query)
    { 
        _logger.LogInformation("Processing {UserId} query {QueryType}", query.From.Id, query.Data);
        
        var card = await _cardOperator.GetCard(query.From.Id);

        if (card is null || !card.CardCreationInProcess)
        {
            card = await _startCardCreation.Handle(query, card);
        }

        await Handle(card, query);
        
        if (NextTask is not null)
        {
            await NextTask.Handle(card, query);
        }
    }
    
    protected abstract Task Handle(Card card, CallbackQuery? query);
}