using System.Text.Json;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.Models;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;

namespace DTR_postcard_bot.AssetManager;

public class Loader(AssetCleaner cleaner, 
    IConfiguration configuration,
    AssetLoader assetLoader, 
    AssetTypeLoader assetTypeLoader,
    TextLoader textLoader,
    CardOperator cardOperator,
    AssetTypeOperator assetTypeOperator,
    ILogger<Loader> logger)
{
    public async Task Execute()
    {
        await cleaner.Execute();

        var dataTypes = GetRequiredDataTypes().ToArray();
        
        await LoadAssets(dataTypes);
        await UpdateCardsInProcess();
        await LoadTexts(dataTypes);
    }

    private async Task LoadTexts(IEnumerable<IConfigurationSection> dataTypes)
    {
        var textJDoc = LoadJson(dataTypes, "PathToTextJson");

        await textLoader.Load(textJDoc);
    }

    private async Task LoadAssets(IEnumerable<IConfigurationSection> dataTypes)
    {
        var assetsJDoc = LoadJson(dataTypes, "PathToAssetJson");

        await assetTypeLoader.Load(assetsJDoc);
        await assetLoader.Load(assetsJDoc);
    }

    private async Task UpdateCardsInProcess()
    {
        var allCurrentCards = await cardOperator.GetAll();
        var cardsArray = allCurrentCards.ToArray();
        
        foreach (var card in cardsArray)
        {
            card.CardCreationInProcess = false;
        }

        await cardOperator.UpdateBatch(cardsArray);
    }

    private IEnumerable<IConfigurationSection> GetRequiredDataTypes()
    {
        var dataTypes = configuration.GetSection("RequiredData").GetChildren();

        return dataTypes;
    }
    
    private JsonDocument LoadJson(IEnumerable<IConfigurationSection> dataTypes, string requestedType)
    {
        var pathToJson = dataTypes
            .FirstOrDefault(dt => dt.Key == requestedType)
            .Value;

        using var stream = File.OpenRead(pathToJson);
        
        return JsonDocument.Parse(stream);
    }
}