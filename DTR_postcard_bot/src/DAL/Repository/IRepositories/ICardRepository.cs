using DTR_postcard_bot.DAL.Models;

namespace DTR_postcard_bot.DAL.Repository.IRepositories;

public interface ICardRepository : IRepository<Card>
{
    Task<bool> BatchRemove(IEnumerable<Card> entities);
}