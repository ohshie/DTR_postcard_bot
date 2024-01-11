using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository.Interfaces;

public interface ITextRepository : IRepository<Text>
{
    Task BatchAdd(IEnumerable<Text> texts);
    Task BatchRemove(IEnumerable<Text> entities);
}