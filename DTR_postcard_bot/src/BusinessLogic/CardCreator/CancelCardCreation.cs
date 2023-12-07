using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.ImageProcessor;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CancelCardCreation(ILogger<CancelCardCreation> logger, CardOperator cardOperator,
        FileCleanUp fileCleanUp, BotMessenger messenger,
        ITextContent textContent, GreetingsKeyboard greetingsKeyboard)
    : CardCreatorBase(logger, cardOperator)
{
    private readonly ILogger<CardCreatorBase> _logger = logger;

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        _logger.LogInformation("Removing all card information from {UserId} and setting state to 0", card.UserId);
        
        fileCleanUp.Execute(card);
        await cardOperator.RemoveCard(card);

        await messenger.UpdateMessageAsync(chatId: card.UserId, 
            text: textContent.ResetMessage(), 
            messageId: card.BotMessagesList.Last(), 
            keyboardMarkup: greetingsKeyboard.CreateKeyboard());
    }
}