namespace DTR_postcard_bot.BusinessLogic.TextContent;

public interface ITextContent
{
    public string GreetingsMessage();
    public string FirstSelectMessage(string element);
    public string ResetMessage();
    public string RequestSomething(string assetType);
    public string CompleteMessage();
}