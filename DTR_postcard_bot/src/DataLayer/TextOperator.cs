using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using DTR_postcard_bot.DataLayer.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer;

public class TextOperator(ITextRepository repository)
{
    public async Task<Text?> GetRequiredText(string trigger)
    {
        var text = await repository.GetAll();
        return await text.FirstOrDefaultAsync(t => t.Trigger == trigger);
    }

    public async Task FillTexts(IEnumerable<Text> allTexts)
    {
        await repository.BatchAdd(allTexts);
    }

    public async Task ClearCurrentTexts()
    {
        var texts = await repository.GetAll();
        await repository.BatchRemove(texts);
    }
}