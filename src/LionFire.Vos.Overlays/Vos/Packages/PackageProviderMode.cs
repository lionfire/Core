namespace LionFire.Vos.Packages
{
    public enum PackageProviderMode
    {
        Unspecified = 0,
        /// <summary>
        /// If true, active packages will be available at PackageActivatorOptions.ActiveSubPath/{PackageName}
        /// </summary>
        Active = 1 << 0,

        /// <summary>
        /// If true, active packages will be overlaid atop one another at PackageActivatorOptions.CombinedSubPath
        /// </summary>
        Overlay = 1 << 1,

        /// <summary>
        /// If true, only a single package will be available at PackageActivatorOptions.ActiveSubPath.
        /// Cannot be used with Active or Overlay.
        /// </summary>
        Single = 1 << 2,
    }
}

