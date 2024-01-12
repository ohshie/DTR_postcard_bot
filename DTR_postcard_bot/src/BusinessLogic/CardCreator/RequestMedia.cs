using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class RequestMedia(ILogger<RequestMedia> logger, 
    StartCardCreation startCardCreation,
    IUnitOfWork unitOfWork,
    AssetChoiceKeyboard assetChoiceKeyboard,
    BotMessenger botMessenger,
    TextContent textContent,
    IMediaBatchHandler mediaBatchHandler,
    TextAssetHandler textAssetHandler) : CardCreatorBase (logger, startCardCreation, unitOfWork)
{
    private InlineKeyboardMarkup? _keyboardMarkup;
    private string _newMessageText = string.Empty;

    protected override async Task Handle(Card card, CallbackQuery query)
    {
        card.Step++;
            
        var requestedAssetType = await ProcessRequest(card, query.Message!.MessageId);

        if (card.Step == 1)
        {
            _keyboardMarkup = await assetChoiceKeyboard.CreateKeyboard(requestedAssetType, firstStep: true);
        }
        else
        {
            _keyboardMarkup = await assetChoiceKeyboard.CreateKeyboard(requestedAssetType);
        }
        
        var assetsToUpdate = await PrepareMessages(card, query.From.Id, requestedAssetType);
        
        await UpdateDb(card, assetsToUpdate);
    }

    private async Task UpdateDb(Card card, IEnumerable<Asset>? assets)
    {
        await unitOfWork.Cards.Update(card);
        if (assets is not null)
        {
            await unitOfWork.Assets.BatchUpdate(assets);
        }
        await unitOfWork.CompleteAsync();
    }

    private async Task<AssetType> ProcessRequest(Card card, int messageId)
    {
        AssetType requestedAssetType;
        
        if (card.Step == 1)
        {
            requestedAssetType = await unitOfWork.AssetTypes.Get(card.AssetTypeIds.First());
            
            await botMessenger.DeleteMessageAsync(card.UserId, messageId);
            
            _newMessageText = await textContent.GetRequiredText("firstSelectMessage", requestedAssetType.Text);
        }
        else
        {
            requestedAssetType = await unitOfWork.AssetTypes.Get(card.AssetTypeIds[card.Step-1]);
            
            _newMessageText = await textContent.GetRequiredText("requestSomething", requestedAssetType.Text);

            await botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);
        }

        return requestedAssetType;
    }

    private async Task<IEnumerable<Asset>?> PrepareMessages(Card card, long chatId, AssetType assetType)
    {
        if (assetType.Type != "text")
        {
            var (tgFileIdExist, mediaContent) = await mediaBatchHandler.PrepareBatch(assetType);
            
            var sentMessagesList = await botMessenger.SendNewMediaGroupMessage(chatId: chatId,
                text: _newMessageText,
                keyboardMarkup: _keyboardMarkup,
                media: mediaContent);
        
            card.BotMessagesList = sentMessagesList.Select(m => m.MessageId).ToList();

            if (!tgFileIdExist)
            {
                var fileIds = sentMessagesList
                    .Where(messages => messages.Photo is not null)
                    .Select(m => m.Photo!.Last())
                    .Select(ps => ps.FileId).ToArray();

                var assetsToUpdate = await unitOfWork.Assets.WriteTelegramFileIds(fileIds, assetType.Type);

                return assetsToUpdate;
            }
        }
        else
        {
            var assetsText = await textAssetHandler.PrepareBatch(assetType);

            var message = await botMessenger.SendTextMessage(chatId: chatId,
                text: assetsText,
                keyboardMarkup: _keyboardMarkup);

            card.BotMessagesList = new List<int>
                { message.MessageId };
        }
        
        return null;
    }
}