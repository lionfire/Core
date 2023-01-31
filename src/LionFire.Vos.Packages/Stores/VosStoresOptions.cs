//using System;
//using LionFire.Vos;
//using LionFire.Vos.Packages;

//namespace LionFire.Vos.Stores
//{
//    public class VosStoresOptions
//    {
//        #region Construction

//        public VosStoresOptions() { }
//        public VosStoresOptions(VobReference storesLocation, PackageManagerOptions packageManagerOptions = null)
//        {
//            StoresLocation = storesLocation;
//            PackageManagerOptions = packageManagerOptions;
//        }

//        public static implicit operator VosStoresOptions(string vosPath) => new VosStoresOptions(vosPath.ToVobReference());
//        public static implicit operator VosStoresOptions(VobReference vobReference) => new VosStoresOptions(vobReference);

//        #endregion

//        public VobReference StoresLocation { get; set; }

//        public PackageManagerOptions PackageManagerOptions { get; set; }
//        public PackageManagerOptions EffectivePackageManagerOptions => PackageManagerOptions ?? PackageManagerOptions.Default;

//        public IVobReference AvailableLocation => StoresLocation?.GetRelativeOrAbsolutePath(EffectivePackageManagerOptions?.AvailableSubPath); 
//    }
//}
