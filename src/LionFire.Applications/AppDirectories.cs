using LionFire.Dependencies;
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
        }
        
        public void Initialize()
        {
            appDir = AppInfo.CustomAppDirName;
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
                return ServiceLocator.GetRequired<AppInfo>().AppDir;
//#if UNITY
//                // TOUNITY TODO: Inject this from UnityHost
//                return UnityEngine.Application.dataPath;
//                //				return UnityEngine.Application.persistentDataPath;
//#else
//                return appDir ??= AppInfo.DetectAppDir();
//#endif
            }
        }
        private string appDir;

        #endregion

        #region Methods

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
        ///// <param name="allUsers"></param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.
        /// </remarks>
        // TODO: Verify permissions, if already exists
        public static void CreateProgramDataFolders(AppInfo appInfo/*, bool allUsers = false*/)
        {
            string companyFolder = appInfo.OrgDir;
            string applicationFolder = appInfo.CustomAppDirName;

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
