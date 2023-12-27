using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using File = System.IO.File;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CompleteAndSendCard : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly AssembleMediaIntoCard _assembleMediaIntoCard;
    private readonly BotMessenger _botMessenger;
    private readonly TextContent _textContent;
    private readonly CardCreationKeyboard _creationKeyboard;

    public CompleteAndSendCard(ILogger<CompleteAndSendCard> logger, 
        CardOperator cardOperator,
        AssembleMediaIntoCard assembleMediaIntoCard,
        BotMessenger botMessenger,
        TextContent textContent,
        CardCreationKeyboard creationKeyboard) : base(logger, cardOperator)
    {
        _cardOperator = cardOperator;
        _assembleMediaIntoCard = assembleMediaIntoCard;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _creationKeyboard = creationKeyboard;
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
    }

    private void FileCleanup(string fileToDelete)
    {
        File.Delete(fileToDelete);
    }
}