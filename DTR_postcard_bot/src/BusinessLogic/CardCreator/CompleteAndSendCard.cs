using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;
using File = System.IO.File;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class CompleteAndSendCard(ILogger<CompleteAndSendCard> logger,
    StartCardCreation startCardCreation,
    IUnitOfWork unitOfWork,
    BotMessenger botMessenger, 
    CardCreationKeyboard creationKeyboard,
    AssembleMediaIntoCard assembleMediaIntoCard,
    TextContent textContent) : CardCreatorBase(logger, startCardCreation, unitOfWork)
{
    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        var createdFile = await assembleMediaIntoCard.Handle(card);
        await botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);

        await botMessenger.SendNewMediaMessage(card.UserId,
            filePath: createdFile,
            text: await textContent.GetRequiredText("completeMessage"),
            keyboardMarkup: creationKeyboard.CreateKeyboard(false));

        FileCleanup(createdFile);
        await CompleteCard(card);
    }

    private async Task CompleteCard(Card card)
    {
        var success =await unitOfWork.Cards.Remove(card);
        success = await unitOfWork.Stats.UpdateOnFinish(card.UserId);
        
        await unitOfWork.CompleteAsync();
    }

    private void FileCleanup(string fileToDelete)
    {
        File.Delete(fileToDelete);
    }
}