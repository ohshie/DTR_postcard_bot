using DTR_postcard_bot.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DataLayer.Repository;

public class TextOperator(IRepository<Text> repository, ILogger<TextOperator> logger)
{
    public async Task<Text> GetRequiredText(string trigger)
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