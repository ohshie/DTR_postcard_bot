using DTR_postcard_bot.DataLayer.Models;
using File = System.IO.File;

namespace DTR_postcard_bot.BusinessLogic.ImageProcessor;

public class FileCleanUp(ILogger<FileCleanUp> logger)
{
    public void Execute(Card card)
    {
        try
        {
            if (File.Exists(card.PathToCurrentCardStep))
            {
                File.Delete(card.PathToCurrentCardStep);
            };
        }
        catch (Exception e)
        {
            logger.LogError("Error removing file located in {FilePath} assigned to {UserId}", card.PathToCurrentCardStep, card.UserId);
            throw;
        }
    }
}