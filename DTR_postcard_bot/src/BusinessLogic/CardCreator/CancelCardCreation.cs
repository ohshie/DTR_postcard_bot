using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CancelCardCreation(ILogger<CancelCardCreation> logger, 
        CardOperator cardOperator, 
        BotMessenger messenger, 
        BotGreetMessage botGreetMessage,
        StartCardCreation startCardCreation)
    : CardCreatorBase(logger, cardOperator, startCardCreation)
{
    private readonly ILogger<CardCreatorBase> _logger = logger;

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        _logger.LogInformation("Removing all card information from {UserId} and setting state to 0", card.UserId);
        
        await cardOperator.RemoveCard(card);

        await messenger.DeleteMessageRangeAsync(chatId: card.UserId,
            messagesId: card.BotMessagesList);

        await botGreetMessage.Send(card.UserId);
    }
}