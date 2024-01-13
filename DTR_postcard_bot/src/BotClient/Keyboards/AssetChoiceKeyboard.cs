using DTR_postcard_bot.BotClient.Keyboards.Buttons;
using DTR_postcard_bot.DAL.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DTR_postcard_bot.BotClient.Keyboards;

public class AssetChoiceKeyboard(ButtonCreator buttonCreator)
{
    public async Task<InlineKeyboardMarkup> CreateKeyboard(AssetType assetType, bool firstStep = false)
    {
        InlineKeyboardMarkup keyboard;
        
        var choiceButtons = await buttonCreator.AssembleChoiceButtons(assetType.Type);

        if (choiceButtons.Length > 4)
        {
            keyboard = AssembleBigKeyboard(choiceButtons);
        }
        else
        {
            keyboard = new InlineKeyboardMarkup(new[]
            {
                choiceButtons
            });
        }
        
        if (!firstStep)
        {
            keyboard = keyboard.InlineKeyboard.Append(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Я передумал, давай начнем заново",
                    callbackData: CallbackList.Cancel)
            }).ToArray();
        }
        
        return keyboard;
    }

    private InlineKeyboardMarkup AssembleBigKeyboard(InlineKeyboardButton[] choiceButtons)
    {
        var splitter = choiceButtons.Length / 2;
        var firstRow = choiceButtons
            .Take(splitter)
            .ToArray();
            
        var secondRow = choiceButtons
            .Skip(splitter)
            .ToArray();

        var bigKeyboardLayout = new[] 
        { 
            firstRow, 
            secondRow 
        };

        return bigKeyboardLayout;
    }
}