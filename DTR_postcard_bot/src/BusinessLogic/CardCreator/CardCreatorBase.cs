using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public abstract class CardCreatorBase
{
    private readonly ILogger<CardCreatorBase> _logger;
    private readonly CardOperator _cardOperator;

    protected CardCreatorBase NextTask;

    protected CardCreatorBase( 
        ILogger<CardCreatorBase> logger, 
        CardOperator cardOperator)
    {
        _logger = logger;
        _cardOperator = cardOperator;
    }
    
    public async Task Execute(CallbackQuery query)
    { 
        _logger.LogInformation("Processing {UserId} query {QueryType}", query.From.Id, query.Data);
        
        var card = await _cardOperator.FetchCard(query.From.Id);
        if(card is null) return;

        await ProcessTask(card, query);
        
        if (NextTask is not null)
        {
            await NextTask.Handle(card, query);
        }
    }

    private async Task ProcessTask(Card card, CallbackQuery query)
    {
        await Handle(card, query);
    }

    protected abstract Task Handle(Card card, CallbackQuery? query);
}