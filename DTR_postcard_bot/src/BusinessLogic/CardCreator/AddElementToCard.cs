using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.ElementsHandler;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class AddElementToCard : CardCreatorBase
{ 
    public AddElementToCard(ILogger<AddElementToCard> logger, 
        CardOperator cardOperator) : 
        base(logger, cardOperator)
    {
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        if (query is null) return;
    }
}