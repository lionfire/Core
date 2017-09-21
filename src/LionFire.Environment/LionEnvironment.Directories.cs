using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LionFire
{
    public partial class LionFireEnvironment
    {
        // .NET Framework Special Folders:
        //  - http://stackoverflow.com/questions/895723/environment-getfolderpath-commonapplicationdata-is-still-returning-c-docum


        public static class Directories
        {
            #region Company

            public static string CompanyProgramDataDir
            {
                get
                {
#if NET461 || NET462
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
#else
                    return @"C:\ProgramData\LionFire\"; // TODO FIXME
#endif
                    //return PlatformServices.Default.Application.
                }
            }


            #endregion

            #region User

            public static string UserProfile
            {
                get
                {
#if NET462 || NET461 || NET451
                    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#else
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        return Environment.GetEnvironmentVariable("USERPROFILE");
                    }
                    else
                    {
                        return Environment.GetEnvironmentVariable("HOME");
                    }
#endif

                }
            }

            #endregion

            #region Logging

            public static string LogsDir
            {
                get
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (CompanyProgramDataDir == null || assembly == null) return null;
                    return Path.Combine(CompanyProgramDataDir, "Logs", assembly.GetName().Name);
                }
            }

            #endregion

            #region (Private) Utility Methods

            internal static void EnsureAllDirectoriesExist()
            {
                foreach (var pi in typeof(LionFireEnvironment.Directories).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    try
                    {
                        var dir = (pi.GetValue(pi) as string);
                        dir.EnsureDirectoryExists();
                    }
                    catch
                    {
                        // EMPTYCATCH TODO
                    }
                }
            }

            #endregion


            #region FUTURE

            #region Windows Store

            // Windows store:
            // http://stackoverflow.com/a/21274767/208304
            // Windows.Storage.ApplicationData.Current.LocalFolder or Windows.Storage.ApplicationData.Current.RoamingFolder

            #endregion

            #endregion

        }
    }
}
