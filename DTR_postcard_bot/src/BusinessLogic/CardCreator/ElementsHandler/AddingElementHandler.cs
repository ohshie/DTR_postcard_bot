using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.ElementsHandler;

public class AddingElementHandler(CardOperator cardOperator, IConfiguration configuration)
{
    public async Task Handle(Card card, CallbackQuery query)
    {
        var typeAndFileArray = query.Data.Split();
        
        var creationStep = Array
            .IndexOf(configuration
                    .GetSection("AssetPaths")
                    .GetChildren()
                    .ToArray(),
            typeAndFileArray
                .First())+1;
        var filePath = typeAndFileArray.Last();
        
        card.PathToCurrentCardStep = filePath;
        card.Step = creationStep;
        
        await cardOperator.UpdateCard(card);
    }
}