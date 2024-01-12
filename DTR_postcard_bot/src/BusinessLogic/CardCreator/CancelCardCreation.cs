using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CancelCardCreation(ILogger<CancelCardCreation> logger, 
        BotMessenger messenger, 
        BotGreetMessage botGreetMessage,
        StartCardCreation startCardCreation, IUnitOfWork unitOfWork)
    : CardCreatorBase(logger, startCardCreation, unitOfWork)
{
    private readonly ILogger<CardCreatorBase> _logger = logger;

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        _logger.LogInformation("Removing all card information from {UserId} and setting state to 0", card.UserId);
        
        await RemoveCard(card);

        await messenger.DeleteMessageRangeAsync(chatId: card.UserId,
            messagesId: card.BotMessagesList);

        await botGreetMessage.Send(card.UserId);
    }

    private async Task RemoveCard(Card card)
    {
        await unitOfWork.Cards.Remove(card);
        await unitOfWork.CompleteAsync();
    }
}