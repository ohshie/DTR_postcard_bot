using DTR_postcard_bot.BusinessLogic.CardCreator;

namespace DTR_postcard_bot.BotClient;

public class CallbackFactory(RequestMedia requestMedia,
    CancelCardCreation cancelCardCreation, AddElementToCard addElementToCard)
{
    private readonly Dictionary<string, Func<CallbackQuery, Task>> _botTaskFactory = new()
    {
        { CallbackList.StartCardCreation, requestMedia.Execute },
        { CallbackList.Cancel, cancelCardCreation.Execute },
        { CallbackList.Add, addElementToCard.Execute },
        { CallbackList.CreateNew, requestMedia.Execute }
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
