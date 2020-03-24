using LionFire.Dependencies;
using LionFire.Structures;
using System;

namespace LionFire.Applications
{
    public class AppInfo
    {
        #region Static

        internal static AppInfo Default = new AppInfo();

        /// <summary>
        /// See also: ServiceLocator.Get&lt;AppInfo&gt;()
        /// </summary>
        public static AppInfo Instance
        {
            get => ManualSingleton<AppInfo>.Instance;
            set
            {
                if (Instance != null) throw new AlreadySetException();
                if (LionFireEnvironment.IsMultiApplicationEnvironment)
                {
                    throw new NotSupportedException("Cannot set AppInfo.Instance when LionFireEnvironment.IsMultiApplicationEnvironment is true.  Use RootInstance instead.");
                }
                ManualSingleton<AppInfo>.Instance = value;
            }
        }

        public static AppInfo RootInstance
        {
            get => rootInstance;
            set
            {
                if (rootInstance != null) throw new AlreadySetException();
                rootInstance = value;
            }
        }
        private static AppInfo rootInstance;

        #endregion

        #region Construction

        public AppInfo()
        {
        }
        public AppInfo(string appName, string orgName = null, string orgDir = null)
        {
            OrgName = orgName;
            AppName = appName;
            this.orgDir = orgDir;
        }

        #endregion

        /// <summary>
        /// Recommendation: no spaces
        /// </summary>
        public string OrgName { get; set; } = "MyOrganization";

        public string OrgDir => orgDir ?? OrgName;
        private string orgDir;

        /// <summary>
        /// Recommended: Globally unique, having your organization in the name.
        /// Recommended: FullName of your program type, or your program's namespace with the program name appended.
        /// Alternate: Reverse domain name style
        /// Default: $"{OrgName}.{AppName.Replace(" ", "")}"
        /// </summary>
        public string AppId
        {
            get => appId ?? $"{OrgName}.{AppName.Replace(" ", "")}";
            set => appId = value;
        }
        private string appId;


        /// <summary>
        /// Recommendations: 
        ///  - unique name within your organization
        ///  - no spaces, unless your executable file has spaces in it.
        /// </summary>
        public string AppName
        {
            get => appName ?? appId;
            set => appName = value;
        }
        private string appName;

        public string ProgramDisplayName
        {
            get => appDisplayName ?? AppName;
            set => appDisplayName = value;
        }
        private string appDisplayName;


        // FUTURE: Allow multiple data dirs
        public string DataDirName { get; set; }
        public string EffectiveDataDirName => DataDirName ?? AppName;

        public string ProgramVersion { get; set; } = "0.0.0";


        public AppDirectories Directories { get; set; }

        /// <summary>
        /// Custom directory name for the application.  Example: c:\ProgramData\{OrgDir}\{CustomAppDir}
        /// </summary>
        public string CustomAppDir { get; set; }
    }
}
