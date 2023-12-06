using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator.ElementsHandler;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class AddElementToCard : CardCreatorBase
{
    private readonly ILogger<AddElementToCard> _logger;
    private readonly AddingElementHandler _addingElementHandler;
    private readonly BotMessenger _botMessenger;
    private readonly ITextContent _textContent;
    private readonly IConfiguration _configuration;
    private readonly AssetChoiceKeyboard _assetChoiceKeyboard;

    public AddElementToCard(ILogger<AddElementToCard> logger, 
        CardOperator cardOperator,
        AddingElementHandler addingElementHandler,
        BotMessenger botMessenger, 
        ITextContent textContent,
        IConfiguration configuration,
        AssetChoiceKeyboard assetChoiceKeyboard) : 
        base(logger, cardOperator)
    {
        _logger = logger;
        _addingElementHandler = addingElementHandler;
        _botMessenger = botMessenger;
        _textContent = textContent;
        _configuration = configuration;
        _assetChoiceKeyboard = assetChoiceKeyboard;
    }

    protected override async Task Handle(Card card, CallbackQuery? query)
    {
        if (query is null) return;

        var contentTypes = FetchTypesFromSettings();
        if (contentTypes is null) return;
        
        IConfigurationSection? nextContentType = null;

        foreach (var contentType in contentTypes)
        {
            if (query.Data.Contains(contentType.Value!))
            {
                await _addingElementHandler.Handle(card, query);
                nextContentType = contentTypes
                    .SkipWhile(ics => ics != contentType)
                    .Skip(1)
                    .FirstOrDefault();
            }
        }

        if (nextContentType is not null)
        {
            var assetType = ContentTypes.TypeDictionary[Array.IndexOf(contentTypes, nextContentType)];
            await _botMessenger.UpdateMessageAsync(chatId: card.UserId,
                messageId: card.LastBotMessageId,
                text: _textContent.AddedSomething(assetType),
                keyboardMarkup: await _assetChoiceKeyboard.CreateKeyboard(nextContentType.Value!));
        }
    }

    private IConfigurationSection[]? FetchTypesFromSettings()
    {
        var contentTypes = _configuration.GetSection("AssetPaths").GetChildren().ToArray();
        if (contentTypes.Length == 0)
        {
            _logger.LogCritical("Content types was not found in appsettings.json");
            return null;
        }

        return contentTypes;
    }
}