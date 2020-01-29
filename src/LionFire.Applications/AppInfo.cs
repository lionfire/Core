namespace LionFire.Applications
{
    public class AppInfo
    {
        internal static AppInfo Default = new AppInfo();

        public AppInfo()
        {
        }
        public AppInfo(string appName, string orgName = null)
        {
            OrgName = orgName;
            AppName = appName;
        }

        /// <summary>
        /// Recommendation: no spaces
        /// </summary>
        public string OrgName { get; set; } = "MyOrganization";

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

        public string ProgramVersion { get; set; } = "0.0.0";
    }
}
