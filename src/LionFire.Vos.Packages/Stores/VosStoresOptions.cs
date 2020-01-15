using System;
using LionFire.Vos;
using LionFire.Vos.Packages;

namespace LionFire.Vos.Stores
{
    public class VosStoresOptions
    {
        #region Construction

        public VosStoresOptions() { }
        public VosStoresOptions(VosReference storesLocation, PackageManagerOptions packageManagerOptions = null)
        {
            StoresLocation = storesLocation;
            PackageManagerOptions = packageManagerOptions;
        }

        public static implicit operator VosStoresOptions(string vosPath) => new VosStoresOptions(vosPath.ToVosReference());
        public static implicit operator VosStoresOptions(VosReference vosReference) => new VosStoresOptions(vosReference);

        #endregion

        public VosReference StoresLocation { get; set; }

        public PackageManagerOptions PackageManagerOptions { get; set; }
        public PackageManagerOptions EffectivePackageManagerOptions => PackageManagerOptions ?? PackageManagerOptions.Default;

        public IVosReference AvailableLocation => StoresLocation?.GetRelativeOrAbsolutePath(EffectivePackageManagerOptions?.AvailableSubPath); 
    }
}
