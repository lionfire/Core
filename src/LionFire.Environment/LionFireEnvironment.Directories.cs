using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SEnvironment = System.Environment;

namespace LionFire
{
    public partial class LionFireEnvironment
    {
        // .NET Framework Special Folders:
        //  - http://stackoverflow.com/questions/895723/environment-getfolderpath-commonapplicationdata-is-still-returning-c-docum

        public static bool AutoCreateDirectories = true;

        public static class Directories
        {

            /// <summary>
            /// TOPORT  Either folder the exe is in, or a parent, if exe is for example nested in a bin folder.
            /// </summary>
            public static string AppDir => throw new NotImplementedException();

            #region Construction

            static Directories()
            {
                if (AutoCreateDirectories) { EnsureAllDirectoriesExist(); }
            }

            #endregion

            public static Dictionary<string, string> Other { get; } = new Dictionary<string, string>();

            #region OS-specific

            #region Windows
            
            /// <summary>
            /// C:\ProgramData
            /// </summary>
            public static string ProgramData
            {
                get
                {
                    if (programData == null)
                    {
                        //SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.CommonApplicationData)
                        programData = SEnvironment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%");
                        if (String.IsNullOrWhiteSpace(programData))
                        {
                            throw new Exception("%ALLUSERSPROFILE% is not set");
                        }
                    }
                    return programData;
                }
                set { programData = value; }
            }
            private static string programData;

            /// <summary>
            /// C:\Users\{username}\AppData\Roaming
            /// </summary>
            public static string RoamingAppData
            {
                get
                {
                    if (appData == null)
                    {
                        var result = SEnvironment.ExpandEnvironmentVariables("%APPDATA%");
                        if (String.IsNullOrWhiteSpace(result))
                        {
                            throw new Exception("%APPDATA% is not set");
                        }
                    }
                    return appData;
                }
                set => appData = value;
            }
            private static string appData;

            /// <summary>
            /// C:\Users\{username}\AppData\Local
            /// </summary>
            public static string LocalAppData // Rename LocalAppDataDir
            {
                get
                {
                    if (localAppData == null)
                    {
                        var result = SEnvironment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
                        if (String.IsNullOrWhiteSpace(result))
                        {
                            throw new Exception("%LOCALAPPDATA% is not set");
                        }
                    }
                    return localAppData;
                }
                set => localAppData = value;
            }
            private static string localAppData;

            #region Windows Store (FUTURE)

            // Windows store:
            // http://stackoverflow.com/a/21274767/208304
            // Windows.Storage.ApplicationData.Current.LocalFolder or Windows.Storage.ApplicationData.Current.RoamingFolder

            #endregion

            #endregion

            #endregion


            /// <summary>
            /// \\Users\{username}  (missing drive letter?)
            /// </summary>
            public static string HomeDir => SEnvironment.ExpandEnvironmentVariables("%HOMEPATH%");

            #region User

            public static string UserProfile
            {
                get
                {
#if NET462 || NET461 || NET451
                    return SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.UserProfile);
#else
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        return SEnvironment.GetEnvironmentVariable("USERPROFILE");
                    }
                    else
                    {
                        return SEnvironment.GetEnvironmentVariable("HOME");
                    }
#endif

                }
            }

            public static string ReleaseNotesDir { get; set; } // TODO

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

        }
    }
}
