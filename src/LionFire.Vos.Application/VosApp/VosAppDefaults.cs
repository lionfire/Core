using LionFire.Vos.Packages;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.VosApp
{
    public class VosAppDefaults
    {
        public static VosAppDefaults Default { get; } = new VosAppDefaults();


        // TODELETE
        //public VosStoresOptions VosStoresOptions { get; } = new VosStoresOptions("/stores", 
        //    new PackageManagerOptions
        //    {
        //        DataLocation = "/~",
        //    });
    }
}
