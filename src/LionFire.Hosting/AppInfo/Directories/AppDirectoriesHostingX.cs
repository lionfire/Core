#nullable enable
using LionFire.Applications;

namespace LionFire.Hosting;

public static class AppDirectoriesHostingX
{
    public static void AutoCreateDirectories(this AppDirectories appDirectories, AppInfo appInfo)
    {
        DirectoryUtils.EnsureAllDirectoriesExist(typeof(LionFireEnvironment.Directories));
        //DirectoryUtils.EnsureAllDirectoriesExist<AppDirectories>();
        AppDirectories.CreateProgramDataFolders(appInfo, appDirectories);
    }

}
