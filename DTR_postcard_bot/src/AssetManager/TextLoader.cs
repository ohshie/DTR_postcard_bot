using System.Text.Json;
using DTR_postcard_bot.DAL.Models;
using DTR_postcard_bot.DAL.UoW.IUoW;

namespace DTR_postcard_bot.AssetManager;

public class TextLoader(IUnitOfWork unitOfWork)
{
    public async Task Load(JsonDocument jDoc)
    {
        var jsonTexts = jDoc.RootElement.EnumerateObject();

        var texts = AssembleIntoBatch(jsonTexts);

        await unitOfWork.Texts.BatchAdd(texts);
    }

    private List<Text> AssembleIntoBatch(JsonElement.ObjectEnumerator allTexts)
    {
        List<Text> texts = new();

        foreach (var textInData in allTexts)
        {
            var text = new Text()
            {
                Trigger = textInData.Name,
                Content = textInData.Value.ToString()
            };
            
            texts.Add(text);
        }

        return texts;
    }
}