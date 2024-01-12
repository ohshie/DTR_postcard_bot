using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public abstract class CardCreatorBase
{
    private readonly ILogger<CardCreatorBase> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StartCardCreation _startCardCreation;

    protected CardCreatorBase? NextTask;

    protected CardCreatorBase( 
        ILogger<CardCreatorBase> logger,
        StartCardCreation startCardCreation, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _startCardCreation = startCardCreation;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(CallbackQuery query)
    { 
        _logger.LogInformation("Processing {UserId} query {QueryType}", query.From.Id, query.Data);
        
        var card = await _unitOfWork.Cards.Get(query.From.Id);

        if (card is null || query.Data == CallbackList.CreateNew)
        {
            card = await _startCardCreation.Handle(query);
        }

        await Handle(card, query);
        
        if (NextTask is not null)
        {
            await NextTask.Handle(card, query);
        }
    }
    
    protected abstract Task Handle(Card card, CallbackQuery query);
}