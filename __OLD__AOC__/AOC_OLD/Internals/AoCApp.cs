

public class AoCApp
{
    
    private static string rootPath;
    public static string RootPath
    {
        get
        {
            // introduce some advanced logic herein - later on
            rootPath ??= File.Exists(".\\session.txt") ? ".\\" : "..\\..\\..\\";
            return rootPath;
        }
    }

}


