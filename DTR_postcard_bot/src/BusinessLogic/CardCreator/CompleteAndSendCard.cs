using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using File = System.IO.File;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CompleteAndSendCard : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly AssembleMediaIntoCard _assembleMediaIntoCard;
    private readonly BotMessenger _botMessenger;
    private readonly TextContent _textContent;
    private readonly CardCreationKeyboard _creationKeyboard;
    private readonly StatOperator _statOperator;

    public CompleteAndSendCard(ILogger<CompleteAndSendCard> logger, 
        CardOperator cardOperator,
        AssembleMediaIntoCard assembleMediaIntoCard,
        BotMessenger botMessenger,
        TextContent textContent,
        CardCreationKeyboard creationKeyboard, 
        StatOperator statOperator,
        StartCardCreation startCardCreation) : base(logger, cardOperator, startCardCreation)
    {
        _cardOperator = cardOperator;
        _assembleMediaIntoCard = assembleMediaIntoCard;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _creationKeyboard = creationKeyboard;
        _statOperator = statOperator;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        var createdFile = await _assembleMediaIntoCard.Handle(card);
        await _botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);

        await _botMessenger.SendNewMediaMessage(card.UserId,
            filePath: createdFile,
            text: await _textContent.GetRequiredText("completeMessage"),
            keyboardMarkup: _creationKeyboard.CreateKeyboard(false));

        FileCleanup(createdFile);
        await _cardOperator.RemoveCard(card);
        
        await _statOperator.RegisterUser(card.UserId, string.Empty);
        await _statOperator.IncrementFinishedCard(card.UserId);
    }

    private void FileCleanup(string fileToDelete)
    {
        File.Delete(fileToDelete);
    }
}