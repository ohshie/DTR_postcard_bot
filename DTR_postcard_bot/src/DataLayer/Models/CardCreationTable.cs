using System.ComponentModel.DataAnnotations;
using File = System.IO.File;

namespace DTR_postcard_bot.DataLayer.Models;

public class Card
{
    [Key]
    public long UserId { get; set; }
    
    public int Step { get; set; }
    public int LastBotMessageId { get; set; }
    public bool CardCreationInProcess { get; set; }
    public string PathToCurrentCardStep { get; set; } = string.Empty;
    public string PathToPreviousCardStep { get; set; } = string.Empty;
}