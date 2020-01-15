using LionFire.Vos.Packages;
using LionFire.Vos.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.VosApp
{
    public class VosAppDefaults
    {
        public static VosAppDefaults Default { get; } = new VosAppDefaults();

        public VosStoresOptions VosStoresOptions { get; } = new VosStoresOptions("/stores", 
            new PackageManagerOptions
            {
                DataLocation = "/~",
            });
    }
}
