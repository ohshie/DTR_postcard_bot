using DTR_postcard_bot.DataLayer.Models;

namespace DTR_postcard_bot.DataLayer.Repository;

public class StatOperator(IRepository<Stat> repository, ILogger<StatOperator> logger)
{
    public async Task RegisterUser(long id, string name)
    {
        var registeredUser = await repository.Get(id);
        if (registeredUser is null)
        {
            logger.LogInformation("Registering user {UserId}", id);

            var newStat = new Stat()
            {
                UserId = id,
                UserName = name
            };

            await repository.Add(newStat);
        }
    }

    public async Task IncrementStartedCard(long id)
    {
        var stat = await repository.Get(id);
        stat.DroppedCards++;
        await repository.Update(stat);
    }

    public async Task IncrementFinishedCard(long id)
    {
        var stat = await repository.Get(id);
        stat.DroppedCards--;
        stat.CreatedCards++;
        await repository.Update(stat);
    }
}