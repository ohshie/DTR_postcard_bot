namespace DTR_postcard_bot.BusinessLogic.TextContent;

public class MzkTextContent : ITextContent
{
    public string GreetingsMessage()
    {
        return
            "Привет! Давай создадим классную рождественскую открытку.\n" +
            "Для этого нажми на кнопку под этим сообщением и следуй моим указаниям.";
    }

    public string FirstSelectMessage(string assetTYpe)
    {
        return $"Отлично, первый шаг - выбор {assetTYpe} для нашей открытки.\n" +
               " Выбери, какой тебе нравится больше!";
    }

    public string RequestSomething(string assetType)
    {
        return $"Прекрасный выбор, далее - {assetType} для нашей открытки.\n" +
               " Укажи, какой тебе нравится больше!";
    }

    public string ResetMessage()
    {
        return "Я готов создать новую открытку для тебя.\n" +
               "Просто нажми кнопку!";
    }

    public string CompleteMessage()
    {
        return "Вау! Посмотри как классно у тебя получилось!\n" +
               "Еще разок?";
    }
}