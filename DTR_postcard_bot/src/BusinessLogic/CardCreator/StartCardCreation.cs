using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.DataLayer;

namespace DTR_postcard_bot.BusinessLogic.CardCreator;

public class StartCardCreation(ILogger<StartCardCreation> logger, 
    ITelegramBotClient botClient,
    CardOperator cardOperator,
    ITextContent textContent,
    BgKeyboard bgKeyboard)
{
    public async Task Handle(CallbackQuery query)
    {
        logger.LogInformation("User initiated Card creation UserId {UserId}", query.From.Id);

        if(await cardOperator.CheckIfExist(query.From.Id)) return;

        await cardOperator.RegisterNewCard(query.From.Id, query.Message.MessageId);

        await botClient.SendTextMessageAsync(chatId: query.From.Id, 
            text: textContent.SelectBgMessage(),
            replyMarkup: await bgKeyboard.CreateKeyboard());
    }
}