namespace DTR_postcard_bot.BusinessLogic.TextContent;

public class MzkTextContent : ITextContent
{
    public string GreetingsMessage()
    {
        return
            "Привет! Давай создадим классную рождественскую открытку.\n" +
            "Для этого нажми на кнопку под этим сообщением и следуй моим указаниям.";
    }

    public string FirstSelectMessage(string element)
    {
        return $"Отлично, первый шаг - выбор {element} для нашей открытки.\n" +
               " Выбери, какой тебе нравится больше!";
    }

    public string AddedSomething(string assetType)
    {
        return $"Прекрасный выбор, далее - {assetType} для нашей открытки.\n" +
               " Укажи, какой тебе нравится больше!";
    }

    public string ResetMessage()
    {
        return "Я готов создать новую открытку для тебя.\n" +
               "Просто нажми кнопку!";
    }
}