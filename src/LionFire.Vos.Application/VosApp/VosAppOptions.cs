using LionFire.Applications;
using LionFire.Dependencies;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LionFire.Vos.VosApp
{
    public class VosAppOptions
    {

        #region (static)

        public static VosAppOptions Current { get; } = DependencyContext.Current.GetService<VosAppOptions>();
        public static VosAppOptions Default { get; } = new VosAppOptions();

        #endregion

        //#region Package Managers // OLD - moving things out of this options class

        //public Dictionary<string, PackageProviderOptions> PackageProviders = new Dictionary<string, PackageProviderOptions>
        //{
        //    [VosAppPackageProviderNames.Core] = new PackageProviderOptions { },
        //    [VosAppPackageProviderNames.Dynamic] = new PackageProviderOptions { },
        //    [VosAppPackageProviderNames.UserData] = new PackageProviderOptions { },
        //};
        
        //#endregion


            // DEPRECATED: UNUSED
        //public bool DefaultMountAppBase = true;
        //public bool DefaultMountActiveDataAtAppBase = true;

        //public bool DefaultMountAppData = true;
        //public bool DefaultMountActiveDataAtAppData = true;

        //public bool DefaultMountVarBase = true;
        //public bool DefaultMountActiveDataAtVarBase = true;

        //public bool DefaultMountVarData = true;
        //public bool DefaultMountActiveDataAtVarData = true;

        public string CustomBaseDir { get; set; }

        public bool EnableLogging { get; set; } = false;

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
            set => userSaveLocation = value;
        }

        private string userSaveLocation;

        #endregion
        //public VosAppOptions VosStoresOptions { get; set; }

        //public bool UsePackages { get; set; }


    }
}
