using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class RequestMedia : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly AssetTypeOperator _assetTypeOperator;
    private readonly BotMessenger _botMessenger;
    private readonly TextContent _textContent;
    private readonly AssetChoiceKeyboard _assetChoiceKeyboard;
    private readonly IMediaBatchHandler _mediaBatchHandler;
    private readonly AssembleMediaIntoCard _assembleMediaIntoCard;
    private readonly CardCreationKeyboard _cardCreationKeyboard;
    private readonly TextAssetHandler _textAssetHandler;

    string _newMessageText = string.Empty;
    InlineKeyboardMarkup _keyboardMarkup;
    IEnumerable<InputMediaPhoto> _mediaContent;
    private AssetType _requestedAssetType;

    public RequestMedia(ILogger<CardCreatorBase> logger, 
        CardOperator cardOperator, 
        AssetTypeOperator assetTypeOperator,
        BotMessenger botMessenger,
        TextContent textContent, 
        AssetChoiceKeyboard assetChoiceKeyboard,
        IMediaBatchHandler mediaBatchHandler,
        AssembleMediaIntoCard assembleMediaIntoCard,
        CardCreationKeyboard cardCreationKeyboard, TextAssetHandler textAssetHandler) : base(logger, cardOperator)
    {
        _cardOperator = cardOperator;
        _assetTypeOperator = assetTypeOperator;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _assetChoiceKeyboard = assetChoiceKeyboard;
        _mediaBatchHandler = mediaBatchHandler;
        _assembleMediaIntoCard = assembleMediaIntoCard;
        _cardCreationKeyboard = cardCreationKeyboard;
        _textAssetHandler = textAssetHandler;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        var assetTypes = await _assetTypeOperator.GetAllAssetTypes();

        // todo need to process this shit better. Make Separate class maybe for it. Because now it sucks dick.
        
        if (card.Step >= assetTypes.Count())
        {
            var createdFile = await _assembleMediaIntoCard.Handle(card);
            await _botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);

            await _botMessenger.SendNewMediaMessage(card.UserId,
                filePath: createdFile,
                text: await _textContent.GetRequiredText("completeMessage"),
                keyboardMarkup: _cardCreationKeyboard.CreateKeyboard(false));

            await _cardOperator.RemoveCard(card);
            return;
        }
        
        card.Step++;
        
        if (card.Step == 1)
        {
            var lastBotMessageId = query.Message.MessageId;
            
            _requestedAssetType = assetTypes.First();
            
            await _botMessenger.DeleteMessageAsync(card.UserId, lastBotMessageId);
            
            _newMessageText = await _textContent.GetRequiredText("firstSelectMessage", _requestedAssetType.Text);
        }
        else
        {
            _requestedAssetType = assetTypes.ElementAtOrDefault(card.Step-1);
            
            _newMessageText = await _textContent.GetRequiredText("requestSomething", _requestedAssetType.Text);

            await _botMessenger.DeleteMessageRangeAsync(card.UserId, card.BotMessagesList);
        }
        
        _keyboardMarkup = await _assetChoiceKeyboard.CreateKeyboard(_requestedAssetType);

        await PrepareMessages(card, query.From.Id);

        await _cardOperator.UpdateCard(card);
    }

    private async Task PrepareMessages(Card card, long chatId)
    {
        List<Message> newMessagesId = new();
        
        if (_requestedAssetType.Type != "text")
        {
            _mediaContent = await _mediaBatchHandler.PrepareBatch(_requestedAssetType);
            
            newMessagesId = await _botMessenger.SendNewMediaGroupMessage(chatId: chatId,
                text: _newMessageText,
                keyboardMarkup: _keyboardMarkup,
                media: _mediaContent);
        
            card.BotMessagesList = newMessagesId.Select(m => m.MessageId).ToList();
        }
        else
        {
            var assetsText = await _textAssetHandler.PrepareBatch(_requestedAssetType);

            var message = await _botMessenger.SendTextMessage(chatId: chatId,
                text: assetsText,
                keyboardMarkup: _keyboardMarkup);

            card.BotMessagesList = new List<int>() { message.MessageId };
        }
    }
}