using LionFire.Referencing;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;

namespace LionFire.Vos.Packages;

public class PackageProviderOptions
{
    /// <summary>
    /// Optional name used to distinguish between multiple PackageManagers in a single Vob tree.
    /// </summary>
    public string Name { get; set; }

    public IReference AvailablePackagesMountTarget { get; }

    public VobMountOptions DefaultMountOptions { get; set; }

    public PackageProviderMode Mode { get; set; }

    /// <summary>
    /// True can be a potential security issue, allowing users to augment/override program data.
    /// Default: null (uses value from PackageProviderDefaults
    /// </summary>
    public bool? IsAutoRegisterAvailablePackagesEnabled { get; set; } = null;

    public bool AutoRegisterPackagesFromSources { get; set; } = true; // TOSECURITY - false if hardened

    /// <summary>
    /// If null, Name will be used by EffectiveAutoAddPackagesFromStoresSubpath
    /// </summary>
    public string AutoAddPackagesFromStoresSubpath { get; set; }
    public string EffectiveAutoAddPackagesFromStoresSubpath => AutoAddPackagesFromStoresSubpath ?? Name;

    #region Subpaths

    // TODO: Make AvailableSubPath IReference, with typical usage a RelativeVobReference?
    public string AvailableSubPath { get; set; } = "available";

    public bool MountEnabledUnderEnabledFolder => Mode.HasFlag(PackageProviderMode.Active);

    /// <summary>
    /// Can be relative or absolute path
    /// </summary>
    // FUTURE: Make this a RelativeOrAbsoluteVobReference?  Is there some way to signify this can be relative or absolute without resorting to documenation?  Variable naming convention?
    public string ActivatedPath { get; set; } = "active";

    /// <summary>
    /// Can be relative or absolute path
    /// </summary>
    public string CombinedPath { get; set; } = "combined";

    #endregion

    #region Derived

    /// <summary>
    /// If false, there will be no overlayed data at the PackageManger's data location
    /// </summary>
    public bool IsDataEnabled => Mode.HasFlag(PackageProviderMode.Overlay);

    #endregion

    #region Whitelist / Blacklist

    public ICollection<string> StoreNameWhitelist { get; set; }
    public ICollection<string> StoreNameBlacklist { get; set; }

    #endregion

    #region FUTURE: Required trust level or other conditions (e.g. PackageInfo.json) for available overlays

    //services.AddPackageProviders("base", new OverlayManagerOptions { StoreNameWhitelist = new List<string> { StoreNames.AppDir } })
    //public int TrustLevel { get; set; }

    #endregion

    #region Construction

    public PackageProviderOptions(PackageProviderMode? mode = null)
    {
        IsAutoRegisterAvailablePackagesEnabled = !LionFireEnvironment.IsHardenedEnvironment;
        if (mode.HasValue) { Mode = mode.Value; }
    }
    public PackageProviderOptions(IReference availablePackagesMountTarget) : this() { AvailablePackagesMountTarget = availablePackagesMountTarget; }

    #endregion

    #region (Static) Defaults

    public static PackageProviderOptions Default { get; } = new PackageProviderOptions
    {
        DefaultMountOptions = VobMountOptions.DefaultRead,
    };
    public bool AutoActivate
    {
        get => false;
        set
        {
            if (value) throw new NotImplementedException($"{nameof(AutoActivate)} = true");
        }
    }

    #endregion
}