namespace DTR_postcard_bot;

public static class Helpers
{
    public static string PathBuilder(string folderType, string subfolder, string? fileName = null)
    {
        string[] path;
        
        if (fileName is not null)
        {
            path = new[] 
            { 
                "app",
                "data",
                folderType, 
                subfolder, 
                fileName 
            };
        }
        else
        {
            path = new[] 
            { 
                "app",
                "data",
                folderType, 
                subfolder, 
            };
        }
        
        return Path.Combine(path);
    }
}