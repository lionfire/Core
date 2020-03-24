using LionFire.Ontology;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LionFire.Applications
{
    public class AppDirectories 
    {
        public AppInfo AppInfo { get; }

        public AppDirectories(AppInfo appInfo)
        {
            AppInfo = appInfo;
            appDir = appInfo.CustomAppDir;

            CreateProgramDataFolders(AppInfo);
        }

        private string CompanyName => AppInfo.OrgName;

        #region Derived

        /// <summary>
        /// C:\Users\{username}\AppData\Local\{OrgName}
        /// </summary>
        public string CompanyLocalAppDataPath => Path.Combine(LionFireEnvironment.Directories.LocalAppData, CompanyName);

        /// <summary>
        /// C:\ProgramData\{CompanyName}
        /// </summary>
        public string CompanyProgramData 
            => string.IsNullOrWhiteSpace(CompanyName) 
            ? null 
            : Path.Combine(LionFireEnvironment.Directories.ProgramData, CompanyName);

        // TODO: Move VosAppHost AppDir finding logic (looking for application.json) into here
        public string AppDir
        {
            get
            {
#if UNITY
                // TOUNITY TODO: Inject this from UnityHost
                return UnityEngine.Application.dataPath;
                //				return UnityEngine.Application.persistentDataPath;
#else
                return appDir ??= DetectAppDir();
#endif
            }
        }
        private string appDir;

        #endregion

        #region Methods

        public string DetectAppDir()
        {
            var result = LionFireEnvironment.AppBinDir;

            string releaseEnding = @"bin\release";
            string debugEnding = @"bin\debug";
            string debugEnding2 = @"dbin";
            string binEnding = @"bin";

            string releaseProjectEnding = @"bin\" + AppInfo.AppName.ToLowerInvariant() + @"\release";
            string debugProjectEnding = @"bin\" + AppInfo.AppName.ToLowerInvariant() + @"\debug";

            if (result.ToLower().EndsWith(releaseEnding))
            {
                result = result.Substring(0, result.Length - releaseEnding.Length);
            }
            else if (result.ToLower().EndsWith(debugEnding))
            {
                result = result.Substring(0, result.Length - debugEnding.Length);
            }
            else if (result.ToLower().EndsWith(debugEnding2))
            {
                result = result.Substring(0, result.Length - debugEnding2.Length);
            }
            else if (result.ToLower().EndsWith(binEnding))
            {
                result = result.Substring(0, result.Length - binEnding.Length);
            }
            else if (result.ToLower().EndsWith(releaseProjectEnding))
            {
                result = Path.Combine(result.Substring(0, result.Length - releaseProjectEnding.Length), AppInfo.AppName);
            }
            else if (result.ToLower().EndsWith(debugProjectEnding))
            {
                result = Path.Combine(result.Substring(0, result.Length - debugProjectEnding.Length), AppInfo.AppName);
            }
            else
            {
                Debug.WriteLine("Could not determine AppDir.  Using AppDir = AppBinDir: " + LionFireEnvironment.AppBinDir);
                result = LionFireEnvironment.AppBinDir;
            }
            //l.Info("AppDir: " + appDir);
            return result;
        }

        /// <summary>
        /// c:\ProgramData\{CompanyName}\{AppDataDirName}
        /// </summary>
        public string AppProgramDataDir => GetProgramDataDir(AppInfo.DataDirName);

        #region (Parameterized) Custom Data Dir in ProgramData

        /// <summary>
        /// c:\ProgramData\{CompanyName}\{namedDataDir}
        /// </summary>
        /// <param name="namedDataDir"></param>
        /// <returns></returns>
        public string GetProgramDataDir(string namedDataDir) => Path.Combine(LionFireEnvironment.Directories.ProgramData, CompanyName, namedDataDir);

        #endregion


        #region Create Program Data Folder

        // Simplifies the creation of folders in the CommonApplicationData folder
        // and setting of permissions for all users.
        // Retrieved from: http://www.codeproject.com/tips/61987/Allow-write-modify-access-to-CommonApplicationData.aspx under CPOL
        
        /// <summary>
        /// Creates a new instance of this class creating the specified company and application folders
        /// if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.
        /// </remarks>
        // TODO: Verify permissions, if already exists
        public static void CreateProgramDataFolders(AppInfo appInfo, bool allUsers = false)
        {
            string companyFolder = appInfo.OrgDir;
            string applicationFolder = appInfo.CustomAppDir;

            // Gets the path of the company's data folder.
            // c:\ProgramData\{CompanyFolder}
            string CompanyFolderPath = appInfo.Directories.CompanyProgramData;

            /// Gets the path of the application's data folder.
            /// c:\ProgramData\{CompanyFolder}\{applicationFolder}
            string ApplicationFolderPath = appInfo.Directories.AppProgramDataDir;

#if MONO || NETSTANDARD
            DirectoryInfo directoryInfo;
            if (!Directory.Exists(CompanyFolderPath))
            {
                Console.WriteLine("Creating " + CompanyFolderPath);
                directoryInfo = Directory.CreateDirectory(CompanyFolderPath);
                //bool modified;
                //directorySecurity = directoryInfo.GetAccessControl();
                //rule = new FileSystemAccessRule(
                //        securityIdentifier,
                //        FileSystemRights.Write |
                //        FileSystemRights.ReadAndExecute |
                //        FileSystemRights.Modify,
                //        AccessControlType.Allow);
                //directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                //directoryInfo.SetAccessControl(directorySecurity);
            }
            if (!Directory.Exists(ApplicationFolderPath))
            {
                Console.WriteLine("Creating " + ApplicationFolderPath);
                directoryInfo = Directory.CreateDirectory(ApplicationFolderPath);
                //if (allUsers)
                {
                    //bool modified;
                    //directorySecurity = directoryInfo.GetAccessControl();
                    //rule = new FileSystemAccessRule(
                    //    securityIdentifier,
                    //    FileSystemRights.Write |
                    //    FileSystemRights.ReadAndExecute |
                    //    FileSystemRights.Modify,
                    //    InheritanceFlags.ContainerInherit |
                    //    InheritanceFlags.ObjectInherit,
                    //    PropagationFlags.InheritOnly,
                    //    AccessControlType.Allow);
                    //directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                    //directoryInfo.SetAccessControl(directorySecurity);
                }
            }
#elif NETFRAMEWORK
            DirectoryInfo directoryInfo;
            DirectorySecurity directorySecurity;
            AccessRule rule;
            SecurityIdentifier securityIdentifier = new SecurityIdentifier
                (WellKnownSidType.BuiltinUsersSid, null);
            if (!Directory.Exists(CompanyFolderPath))
            {
                directoryInfo = Directory.CreateDirectory(CompanyFolderPath);
                bool modified;
                directorySecurity = directoryInfo.GetAccessControl();
                rule = new FileSystemAccessRule(
                        securityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        AccessControlType.Allow);
                directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                directoryInfo.SetAccessControl(directorySecurity);
            }
            if (!Directory.Exists(ApplicationFolderPath))
            {
                directoryInfo = Directory.CreateDirectory(ApplicationFolderPath);
                if (allUsers)
                {
                    bool modified;
                    directorySecurity = directoryInfo.GetAccessControl();
                    rule = new FileSystemAccessRule(
                        securityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        InheritanceFlags.ContainerInherit |
                        InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow);
                    directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                    directoryInfo.SetAccessControl(directorySecurity);
                }
            }
#else
            throw new NotImplementedException();
#endif
        }

        #endregion

        #endregion
    }
}
