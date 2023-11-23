public class AppConfig
{
    public static string DownloadDir => Path.Combine(Path.GetTempPath(), "LionFire.Orleans.Cli", "Cache");

}
