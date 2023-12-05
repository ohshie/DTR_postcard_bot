using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.ImageProcessor;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CancelCardCreation(ILogger<CancelCardCreation> logger, CardOperator cardOperator,
        FileCleanUp fileCleanUp, ITelegramBotClient botClient,
        ITextContent textContent, GreetingsKeyboard greetingsKeyboard)
    : CardCreatorBase(logger, cardOperator)
{
    private readonly ILogger<CardCreatorBase> _logger = logger;

    protected override async Task Handle(Card card)
    {
        _logger.LogInformation("Removing all card information from {UserId} and setting state to 0", card.UserId);
        
        fileCleanUp.Execute(card);
        await cardOperator.RemoveCard(card);

        await botClient.SendTextMessageAsync(chatId: card.UserId, text: textContent.ResetMessage(),
            replyMarkup: greetingsKeyboard.CreateKeyboard());
    }
}