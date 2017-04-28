using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
    public class AppInfo : IInitializable
    {

        public string CompanyName { get; set; }
        public string ProgramName { get; set; }
        public string AppDataDirName { get; set; }

        #region Construction

        public AppInfo() { }
        public AppInfo(string companyName, string programName, string appDataDirName)
        {
            this.CompanyName = companyName;
            this.ProgramName = programName;
            this.AppDataDirName = appDataDirName;
        }
        
        #endregion

        public Task<bool> Initialize()
        {
            LionFireEnvironment.CompanyName = CompanyName;
            LionFireEnvironment.ProgramName = ProgramName;
            LionFireEnvironment.AppDataDirName = AppDataDirName;
            return Task.FromResult(true);
        }

    }
    //public static class AppInfoExtensions
    //{
    //    public static IAppHost SetAppInfo(this IAppHost app, string companyName, string programName, string appDataDirName)
    //    {
    //        LionFireEnvironment.CompanyName = companyName;
    //        LionFireEnvironment.ProgramName = programName
    //    }
    //}
}
