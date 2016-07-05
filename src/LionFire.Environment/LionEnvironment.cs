using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using LionFire.ExtensionMethods;

namespace LionFire
{
    public class LionEnvironment
    {
//#if NET451
//            Console.WriteLine($"Hello {string.Join(" ", args)} From .NETFramework,Version=v4.5.1");
//#elif NETCOREAPP1_0
//            Console.WriteLine($"Hello {string.Join(" ", args)} From .NETCoreApp,Version=v1.0");
//#endif

        #region Windows Store

        // Windows store:
        // http://stackoverflow.com/a/21274767/208304
        // Windows.Storage.ApplicationData.Current.LocalFolder or Windows.Storage.ApplicationData.Current.RoamingFolder

        #endregion

        public static string CompanyProgramDataDir {
            get {
                //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                //return PlatformServices.Default.Application.
                return @"C:\ProgramData\LionFire\"; // TODO FIXME
            }
        }

        static LionEnvironment()
        {

            //PlatformServices.Default.Application.RuntimeFramework
            

            LogDir.EnsureDirectoryExists();
        }

        private static void EnsureDirectoryExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static string LogDir { get { return Path.Combine(CompanyProgramDataDir, "Logs", Assembly.GetEntryAssembly().GetName().Name); } }

        
    }
}
