using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DTR_postcard_bot.BotClient;

public class BotClient(ITelegramBotClient botClient, 
    ILogger<BotClient> logger, BotGreetMessage botGreetMessage,
    CallbackFactory callbackFactory)
{
    private static readonly ManualResetEvent ShutdownEvent = new ManualResetEvent(false);

    public async Task BotOperations()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);
        
        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
        
        logger.LogWarning("bot started @{Me}", me);
        
        ShutdownEvent.WaitOne();
        logger.LogCritical("bot stopped");
        
        cts.Cancel();
    }
    
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogWarning("Handling update {Update} type {Type}", update.Id, update.Type);
        
        if (update.CallbackQuery is { } callbackQuery)
        {
            await callbackFactory.CallBackDataManager(callbackQuery);
            return;
        }
        
        if (update.Message is not {} message) return;

        await botGreetMessage.Send(message);
    }
    
    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        try
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Error while processing update: {Exception}", ex);
            return Task.CompletedTask;
        }
    }
}