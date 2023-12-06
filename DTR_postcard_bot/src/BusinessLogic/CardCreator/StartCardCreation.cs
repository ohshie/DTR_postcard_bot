using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;
using Microsoft.Extensions.Configuration;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger,
    CardOperator cardOperator,
    ITextContent textContent,
    AssetChoiceKeyboard assetChoiceKeyboard,
    BotMessenger botMessenger,
    IConfiguration configuration)
{
    public async Task Handle(CallbackQuery query)
    {
        logger.LogInformation("User initiated Card creation UserId {UserId}", query.From.Id);

        if(await cardOperator.CheckIfExist(query.From.Id)) return;

        await cardOperator.RegisterNewCard(query.From.Id, query.Message.MessageId);

        await botMessenger.UpdateMessageAsync(chatId: query.From.Id, messageId: query.Message.MessageId,
            text: textContent.SelectBgMessage(),
            keyboardMarkup: await assetChoiceKeyboard.CreateKeyboard(configuration
                .GetSection("AssetPaths")
                .GetChildren()
                .First().Value!));
    }
}