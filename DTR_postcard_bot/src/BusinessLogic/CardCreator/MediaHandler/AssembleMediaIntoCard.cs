using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;

namespace DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;

public class AssembleMediaIntoCard(AssetOperator assetOperator)
{
    private readonly string _assetsFolderName = "assets";
    private readonly Random _random = new();
    private string _pathToCreatedFile = string.Empty;
    
    public async Task<string> Handle(Card card)
    {
        List<Image> images = new();

        foreach (var step in card.CreationSteps)
        {
            var typeAndId = step.Split(" ");
            var asset = await LoadImages(typeAndId.First(), long.Parse(typeAndId.Last()));
            images.Add(asset);
        }

        await ImageManipulation(images, card);

        return _pathToCreatedFile;
    }

    private async Task<Image> LoadImages(string type, long id)
    {
        var assetName = await assetOperator.Get(id);
        var pathToAsset = Helpers.PathBuilder(folderType: _assetsFolderName,
            subfolder: type, 
            fileName: assetName.FileName);

        return await Image.LoadAsync(pathToAsset);
    }

    private async Task ImageManipulation(IEnumerable<Image> images, Card card)
    {
        const int width = 720;
        const int height = 720;

        using (var canvas = new Image<Rgba32>(width, height))
        {
            canvas.Mutate(x =>
            {
                foreach (var image in images)
                {
                    var offsetX = (width - image.Width) / 2;
                    var offsetY = (height - image.Height) / 2;

                    x.DrawImage(image, new Point(offsetX, offsetY), 1f);
                }
            });

            _pathToCreatedFile = Helpers.PathBuilder(folderType: "output", 
                subfolder: card.UserId.ToString(),
                fileName: _random.Next(999)+".jpg");
            
            Directory.CreateDirectory(Helpers.PathBuilder(folderType: "output", 
                subfolder: card.UserId.ToString()));
            
            await canvas.SaveAsync(_pathToCreatedFile);
        }

        foreach (var image in images)
        {
            image.Dispose();
        }
    }
}