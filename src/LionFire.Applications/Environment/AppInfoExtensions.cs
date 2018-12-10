
namespace LionFire.Applications.Hosting
{
    public static class AppInfoExtensions
    {
        // FUTURE ENH: IComposableAdding and IReplaces, not best us
        public static IAppHost AppInfo(this IAppHost app, AppInfo appInfo)
            //string companyName, string programName, string appDataDirName)
        {
            app.AddSingleton(appInfo);
            //app.Add(appInfo);
            return app;
            //LionFireEnvironment.CompanyName = companyName;
            //LionFireEnvironment.ProgramName = programName
        }
    }
}
