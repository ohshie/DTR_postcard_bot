namespace DTR_postcard_bot.BotClient;

public static class CallbackList
{
    private const string Slash = "/";
    private const string Space = " ";
    
    public const string StartCardCreation = Slash + "start_new_card";
    public const string Add = Slash + "add";
    public const string Cancel = Slash + "cancel";
}