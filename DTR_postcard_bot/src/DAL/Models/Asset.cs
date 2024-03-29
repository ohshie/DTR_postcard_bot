using System.ComponentModel.DataAnnotations;

namespace DTR_postcard_bot.DAL.Models;

public class Asset
{
    [Key]
    public long Id { get; set; }
    public AssetType Type { get; set; } = new();
    public string Channel { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string TelegramFileId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool OutputAsset { get; set; }
    public bool DisplayAsset { get; set; }
}