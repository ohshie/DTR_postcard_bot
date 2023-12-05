using System.ComponentModel.DataAnnotations;

namespace DTR_postcard_bot.DataLayer.Models;

public class Asset
{
    [Key]
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}