using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.ElementsHandler;

public class RequestMedia : CardCreatorBase
{
    private readonly CardOperator _cardOperator;
    private readonly AssetTypeOperator _assetTypeOperator;
    private readonly BotMessenger _botMessenger;
    private readonly ITextContent _textContent;
    private readonly AssetChoiceKeyboard _assetChoiceKeyboard;
    private readonly MediaPrepareHandler _mediaPrepareHandler;

    public RequestMedia(ILogger<CardCreatorBase> logger, 
        CardOperator cardOperator, 
        AssetTypeOperator assetTypeOperator,
        BotMessenger botMessenger,
        ITextContent textContent, 
        AssetChoiceKeyboard assetChoiceKeyboard,
        MediaPrepareHandler mediaPrepareHandler) : base(logger, cardOperator)
    {
        _cardOperator = cardOperator;
        _assetTypeOperator = assetTypeOperator;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _assetChoiceKeyboard = assetChoiceKeyboard;
        _mediaPrepareHandler = mediaPrepareHandler;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        var lastBotMessageId = query.Message.MessageId;
        
        var assetTypes = await _assetTypeOperator.GetAllAssetTypes();

        if (card.Step >= assetTypes.Count()) return;

        card.Step++;
        
        var newMessageText = _textContent.FirstSelectMessage(assetTypes.First().Text);
        var keyboardMarkup = await _assetChoiceKeyboard.CreateKeyboard(assetTypes.First());
        var mediaContent = await _mediaPrepareHandler.PrepareBatch(assetTypes.First());

        await _botMessenger.DeleteMessageAsync(card.UserId, lastBotMessageId);
        var newMessagesId = await _botMessenger.SendNewMediaMessage(chatId: query.From.Id,
            text: newMessageText,
            keyboardMarkup: keyboardMarkup,
            media: mediaContent);
        
        card.BotMessagesList = newMessagesId.Select(m => m.MessageId).ToList();

        await _cardOperator.UpdateCard(card);
    }
}