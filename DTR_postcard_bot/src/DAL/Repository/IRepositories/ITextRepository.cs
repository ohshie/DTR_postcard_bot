using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface ITextRepository : IRepository<Text>
{
    Task<string> GetByTrigger(string trigger);
    Task<bool> BatchAdd(IEnumerable<Text> texts);
    Task<bool> RemoveAll();
    Task<bool> BatchRemove(IEnumerable<Text> texts, IEnumerable<Text>? allTexts = null);
}