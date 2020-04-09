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
        public static class Platform
        {
            // TODO.  Use Platform enum 
            public static bool IsWindows => true;
            public static bool IsLinux => true;
            public static bool IsMac => true;
            public static bool IsAndroid => true;
            public static bool IsUnix => IsLinux || IsMac || IsAndroid;
        }

        // .NET Framework Special Folders:
        //  - http://stackoverflow.com/questions/895723/environment-getfolderpath-commonapplicationdata-is-still-returning-c-docum

        public static bool AutoCreateDirectories = true;


        public static class Directories
        {
            //public static Func<string, bool?> IsTempDirectory = path => // TODO?
            // TOSECURITY - Hardening - if IsHardened mode, default these Is__Directory to null to force explicit initialization.
            public static Func<string, bool?> IsVariableDirectory = path =>
            {
                // REVIEW: Move to platform-specific DLLs?
                if (LionFireEnvironment.Platform.IsWindows)
                {
                    if (path.Length > 1 && path[1] == ':')
                    {
                        path = path.Substring(2);
                    }

                    if (path.PathEqualsOrIsDescendant(@"\Program Files") || path.PathEqualsOrIsDescendant(@"\Program Files (x86)")) { return false; }
                    if (path.PathEqualsOrIsDescendant(@"\ProgramData") { return true; }

                    // Note: executables may be installed to \Users\user\LocalAppData -- these are considered Variable, though an application may wish to treat it as not.

                    return false; 
                }
                if (LionFireEnvironment.Platform.IsUnix)
                {
                    if (path.PathEqualsOrIsDescendant("/var")) { return true; }
                    if (path.PathEqualsOrIsDescendant("/tmp")) { return true; }
                    if (path.PathEqualsOrIsDescendant("/usr")) { return false; }

                }
                return null;
            };
            public static Func<string, bool?> IsUserDirectory = path =>
            {
                if (LionFireEnvironment.Platform.IsWindows)
                {
                    if (path.Length > 1 && path[1] == ':')
                    {
                        path = path.Substring(2);
                    }
                    if (path.StartsWith(@"\Users\"))
                    {
                        var split = path.Split('\\');
                        if (split.Length == 1) return false;
                        if (split[1] == "Public" || split[1] == "All Users" || split[1] == "inetpub") { return false; }

                        return true;
                    }
                }
                if (LionFireEnvironment.Platform.IsUnix)
                {
                    if (path.StartsWith("/home"))
                    {
                        var split = path.Split('/');
                        return split.Length > 1;
                    }
                    else if (path.StartsWith("/var/home"))
                    {
                        var split = path.Split('/');
                        return split.Length > 2;
                    }
                }
                return null;
            };

            /// <summary>
            /// TOPORT  Either folder the exe is in, or a parent, if exe is for example nested in a bin folder.
            /// MOVE to AppInfo
            /// </summary>
            public static string AppDir => throw new NotImplementedException();

            #region Construction

            static Directories()
            {
                if (AutoCreateDirectories) { EnsureAllDirectoriesExist(); }
            }

            #endregion

            public static Dictionary<string, string> Other { get; } = new Dictionary<string, string>();

            /// <summary>
            /// C:\ProgramData:
            ///  - Windows 10
            ///  - Windows 7 (64-bit)
            ///  - Windows Vista
            ///  
            /// C:\Documents and Settings\All Users\Application Data
            ///  - Windows XP
            ///  - Windows Server 2003
            ///  
            /// /usr/share:
            ///  - Ubuntu 16.04 using dotnet core (3.0.100)
            ///  - Output on Ubuntu 16.04 with mono 4.2.1
            ///  - Android 6 using Xamarin 7.2
            ///  - iOS Simulator 10.3 using Xamarin 7.2
            /// 
            /// ?:
            ///  - ipad 10.3 using Xamarin 7.2
            ///  - ipad 13.3 using Xamarin 16.4
            /// </summary>
            //private static readonly string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

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
                set => programData = value;
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
