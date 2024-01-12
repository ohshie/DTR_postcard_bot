using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot;

public class TextContent(IUnitOfWork unitOfWork)
{
    public async Task<string> GetRequiredText(string trigger)
    {
        var textBase = await unitOfWork.Texts.GetByTrigger(trigger);

        if (textBase!.Contains("{assetType}"))
        {
            return textBase.Replace("{assetType}", trigger);
        }

        return textBase;
    }
    
    public async Task<string> GetRequiredText(string trigger, string assetType)
    {
        var textBase = await unitOfWork.Texts.GetByTrigger(trigger);

        if (textBase!.Contains("{assetType}"))
        {
            return textBase.Replace("{assetType}", assetType);
        }

        return textBase;
    }
}