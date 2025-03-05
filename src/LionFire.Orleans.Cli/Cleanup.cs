using ConsoleAppFramework;
using static System.Console;

[RegisterCommands("cleanup")]
public class Cleanup //: ConsoleAppBase
{
    //[RootCommand]
    //[Command("cleanup", "Clean download cache")]
    [Command("")]
    
    public void Run()
    {
        if (Directory.Exists(AppConfig.DownloadDir))
        {
            WriteLine("Cleaning up cache dir: " + AppConfig.DownloadDir);
            Directory.Delete(AppConfig.DownloadDir, true);
        } else
        {
            WriteLine("Already cleaned up");
        }
    }
}
