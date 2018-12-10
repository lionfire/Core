using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    public static class VosConfiguration
    {
        public static string CustomBaseDir { get; set; }

        /// <summary>
        /// This is a convenience for developers to store user-data in an alternate location
        /// (other than C:\ProgramData\..., so that it can be saved in version control.
        /// </summary>
        public static string CustomDataDir { get; set; }

        //public static string UserSaveLocation = VosLocationNames.CustomData ?? VosLocationNames.VarData;
        
        #region UserSaveLocation

        public static string UserSaveLocation
        {
            get {
                if (userSaveLocation != null) return userSaveLocation;
                
                if (CustomDataDir != null) return VosStoreNames.CustomData;
                
                return VosStoreNames.VarData;

                //return userSaveLocation ?? VosStoreNames.CustomData ?? VosStoreNames.VarData;
            }
            set { userSaveLocation = value; }
        } private static string userSaveLocation;

        #endregion

        /// <summary>
        /// If true, Readonly mounts are registered as writable mounts and context is checked at write time.
        /// </summary>
        public static readonly bool AllowIsReadonlyOverride = true;
    }
}
