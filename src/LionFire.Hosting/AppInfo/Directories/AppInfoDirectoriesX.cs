using LionFire.Applications;

namespace LionFire.Hosting;

public static class AppInfoDirectoriesX
{
    public static AppDirectories GetDefaultDirectories(this AppInfo appInfo)
    {
        var d = new AppDirectories(appInfo);
        d.Initialize();
        return d;
    }
}
