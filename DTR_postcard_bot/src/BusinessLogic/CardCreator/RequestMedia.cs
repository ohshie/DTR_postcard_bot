using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class RequestMedia : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly BotMessenger _botMessenger;
    private readonly TextContent _textContent;
    private readonly AssetChoiceKeyboard _assetChoiceKeyboard;
    private readonly IMediaBatchHandler _mediaBatchHandler;
    private readonly TextAssetHandler _textAssetHandler;
    private readonly AssetOperator _assetOperator;
    private readonly AssetTypeOperator _assetTypeOperator;

    private string _newMessageText = string.Empty;
    private InlineKeyboardMarkup _keyboardMarkup = null!;

    public RequestMedia(ILogger<RequestMedia> logger, 
        CardOperator cardOperator, 
        BotMessenger botMessenger,
        TextContent textContent, 
        AssetChoiceKeyboard assetChoiceKeyboard,
        IMediaBatchHandler mediaBatchHandler,
        TextAssetHandler textAssetHandler,
        AssetOperator assetOperator,
        AssetTypeOperator assetTypeOperator,
        StartCardCreation startCardCreation) : base(logger, cardOperator, startCardCreation)
    {
        _cardOperator = cardOperator;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _assetChoiceKeyboard = assetChoiceKeyboard;
        _mediaBatchHandler = mediaBatchHandler;
        _textAssetHandler = textAssetHandler;
        _assetOperator = assetOperator;
        _assetTypeOperator = assetTypeOperator;
    }

    protected override async Task Handle(Card card, CallbackQuery query)
    {
        card.Step++;
            
        var requestedAssetType = await ProcessRequest(card, query.Message!.MessageId);

        if (card.Step == 1)
        {
            _keyboardMarkup = await _assetChoiceKeyboard.CreateKeyboard(requestedAssetType, firstStep: true);
        }
        else
        {
            _keyboardMarkup = await _assetChoiceKeyboard.CreateKeyboard(requestedAssetType);
        }
        
        await PrepareMessages(card, query.From.Id, requestedAssetType);

        await _cardOperator.UpdateCard(card);
    }

    private async Task<AssetType> ProcessRequest(Card card, int messageId)
    {
        AssetType requestedAssetType;
        
        if (card.Step == 1)
        {
            requestedAssetType = await _assetTypeOperator.GetById(card.AssetTypeIds.First());
            
            await _botMessenger.DeleteMessageAsync(card.UserId, messageId);
            
            _newMessageText = await _textContent.GetRequiredText("firstSelectMessage", requestedAssetType.Text);
        }
        else
        {
            requestedAssetType = await _assetTypeOperator.GetById(card.AssetTypeIds[card.Step-1]);
            
            _newMessageText = await _textContent.GetRequiredText("requestSomething", requestedAssetType.Text);

            await _botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);
        }

        return requestedAssetType;
    }

    private async Task PrepareMessages(Card card, long chatId, AssetType assetType)
    {
        if (assetType.Type != "text")
        {
            var (tgFileIdExist, mediaContent) = await _mediaBatchHandler.PrepareBatch(assetType);
            
            var sentMessagesList = await _botMessenger.SendNewMediaGroupMessage(chatId: chatId,
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

                await _assetOperator.WriteTelegramFileIds(fileIds, assetType.Type);
            }
        }
        else
        {
            var assetsText = await _textAssetHandler.PrepareBatch(assetType);

            var message = await _botMessenger.SendTextMessage(chatId: chatId,
                text: assetsText,
                keyboardMarkup: _keyboardMarkup);

            card.BotMessagesList = new List<int>
                { message.MessageId };
        }
    }
}