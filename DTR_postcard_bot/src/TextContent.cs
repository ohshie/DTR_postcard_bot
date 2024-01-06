using DTR_postcard_bot.DataLayer;

namespace DTR_postcard_bot;

public class TextContent(TextOperator textOperator)
{
    public async Task<string> GetRequiredText(string trigger)
    {
        var textBase = await textOperator.GetRequiredText(trigger);

        if (textBase!.Content.Contains("{assetType}"))
        {
            return textBase.Content.Replace("{assetType}", trigger);
        }

        return textBase.Content;
    }
    
    public async Task<string> GetRequiredText(string trigger, string assetType)
    {
        var textBase = await textOperator.GetRequiredText(trigger);

        if (textBase!.Content.Contains("{assetType}"))
        {
            return textBase.Content.Replace("{assetType}", assetType);
        }

        return textBase.Content;
    }
}