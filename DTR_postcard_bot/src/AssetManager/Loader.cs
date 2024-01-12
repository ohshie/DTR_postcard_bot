using System.Text.Json;
using DTR_postcard_bot.DAL.UoW.IUoW;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;

namespace DTR_postcard_bot.AssetManager;

public class Loader(AssetCleaner cleaner,
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    AssetLoader assetLoader, 
    AssetTypeLoader assetTypeLoader,
    TextLoader textLoader,
    ILogger<Loader> logger)
{
    public async Task<bool> Execute()
    {
        logger.LogInformation("Initializing assets");
        await cleaner.Execute();

        var dataTypes = GetRequiredDataTypes().ToArray();
        
        var assetsSuccess = await LoadAssets(dataTypes);
        var textSuccess = await LoadTexts(dataTypes);
        
        await unitOfWork.CompleteAsync();
        
        return assetsSuccess && textSuccess;
    }

    private async Task<bool> LoadTexts(IEnumerable<IConfigurationSection> dataTypes)
    {
        var textJDoc = LoadJson(dataTypes, "PathToTextJson");

        if (textJDoc is null) return false;

        await textLoader.Load(textJDoc);
        return true;
    }

    private async Task<bool> LoadAssets(IEnumerable<IConfigurationSection> dataTypes)
    {
        var assetsJDoc = LoadJson(dataTypes, "PathToAssetJson");

        if (assetsJDoc is null) return false;
    
        var assetTypes = await assetTypeLoader.Load(assetsJDoc);
        await assetLoader.Load(assetsJDoc, assetTypes);
        
        return true;
    }
    
    private IEnumerable<IConfigurationSection> GetRequiredDataTypes()
    {
        var dataTypes = configuration.GetSection("RequiredData").GetChildren();

        return dataTypes;
    }
    
    private JsonDocument? LoadJson(IEnumerable<IConfigurationSection> dataTypes, string requestedType)
    {
        try
        {
            var pathToJson = dataTypes
                .FirstOrDefault(dt => dt.Key == requestedType)?
                .Value;
            if (string.IsNullOrEmpty(pathToJson)) return null;
            
            using var stream = File.OpenRead(pathToJson);
            return JsonDocument.Parse(stream);

        }
        catch (Exception e)
        {
            logger.LogError("Something went wrong when loading Assets JSON file {Exception}", e.Message);
            throw;
        }
    }
}