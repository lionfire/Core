using LionFire.Applications;

namespace LionFire.Vos.VosApp
{
    public class VosAppOptions
    {
        public static VosAppOptions Default { get; } = new VosAppOptions();


        public AppInfo AppInfo { get; set; }


        #region Package Managers

        public bool UseDataPackageMangaer = true;
        public bool UseBasePackageMangaer = true;

        #endregion

        public bool DefaultMountAppBase = true;
        public bool DefaultMountActiveDataAtAppBase = true;

        public bool DefaultMountAppData = true;
        public bool DefaultMountActiveDataAtAppData = true;

        public bool DefaultMountVarBase = true;
        public bool DefaultMountActiveDataAtVarBase = true;

        public bool DefaultMountVarData = true;
        public bool DefaultMountActiveDataAtVarData = true;

        public string CustomBaseDir { get; set; }

        /// <summary>
        /// This is a convenience for developers to store user-data in an alternate location
        /// (other than C:\ProgramData\..., so that it can be saved in version control.
        /// </summary>
        public string CustomDataDir { get; set; }

        #region UserSaveLocation

        public string UserSaveLocation
        {
            get
            {
                if (userSaveLocation != null) return userSaveLocation;

                if (CustomDataDir != null) return VosStoreNames.CustomData;

                return VosStoreNames.VarData;

                //return userSaveLocation ?? VosStoreNames.CustomData ?? VosStoreNames.VarData;
            }
            set { userSaveLocation = value; }
        }

        public bool UseExeDirAsAppDirIfMissing { get; set; }

        private string userSaveLocation;

        #endregion
        //public VosAppOptions VosStoresOptions { get; set; }

        //public bool UsePackages { get; set; }
    }
}
