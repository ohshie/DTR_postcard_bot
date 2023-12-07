using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTR_postcard_bot.DataLayer.Models;

public class Asset
{
    [Key]
    public long Id { get; set; }
    public AssetType? Type { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}