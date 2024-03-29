using System.ComponentModel.DataAnnotations;

namespace DTR_postcard_bot.DAL.Models;

public class Card
{
    [Key]
    public long UserId { get; set; }
    public int Step { get; set; }
    public bool CardCreationInProcess { get; set; }
    public List<int> BotMessagesList { get; set; } = new();
    public List<string> CreationSteps { get; set; }= new();

    public List<int> AssetTypeIds { get; set; } = new();
}