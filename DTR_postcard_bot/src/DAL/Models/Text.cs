namespace DTR_postcard_bot.DAL.Models;

public class Text
{
    public long Id { get; set; }
    public string Trigger { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}