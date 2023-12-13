using DTR_postcard_bot.BusinessLogic.CardCreator;

namespace DTR_postcard_bot.BotClient;

public class CallbackFactory(StartCardCreation startCardCreation,
    CancelCardCreation cancelCardCreation, AddElementToCard addElementToCard)
{
    private readonly Dictionary<string, Func<CallbackQuery, Task>> _botTaskFactory = new()
    {
        { CallbackList.StartCardCreation, startCardCreation.Handle },
        { CallbackList.Cancel, cancelCardCreation.Execute },
        { CallbackList.Add, addElementToCard.Execute },
        { CallbackList.CreateNew, startCardCreation.Handle }
    };
    
    public async Task CallBackDataManager(CallbackQuery query)
    {
        string key = query.Data.Split(" ")[0];
        
        if (query.Data != null && _botTaskFactory.TryGetValue(key, out var value))
        {
            await value(query);
        }
    }
}
