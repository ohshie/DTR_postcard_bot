namespace DTR_postcard_bot.DataLayer.Models;

public class Stat
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int CreatedCards { get; set; }
    public int DroppedCards { get; set; }
}