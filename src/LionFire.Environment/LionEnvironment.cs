using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace LionFire
{
    public class LionEnvironment
    {
        public static string CompanyProgramDataDir {
            get {
                //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                //return PlatformServices.Default.Application.
                return @"C:\ProgramData\LionFire\"; // TODO FIXME
            }
        }

        static LionEnvironment()
        {
            LogDir.EnsureDirectoryExists();
        }

        private static void EnsureDirectoryExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static string LogDir { get { return Path.Combine(CompanyProgramDataDir, "Logs", Assembly.GetEntryAssembly().FullName); } } 
    }
}
