using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Task BatchRemove(IEnumerable<Card> entities);
}