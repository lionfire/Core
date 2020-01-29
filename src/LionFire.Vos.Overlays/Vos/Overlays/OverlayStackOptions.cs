using LionFire.Referencing;
using LionFire.Vos.Mounts;
using System.Collections.Generic;

namespace LionFire.Vos.Overlays
{

    public class OverlayStackOptions
    {
        /// <summary>
        /// Optional name used to distinguish between multiple PackageManagers in a single Vob tree.
        /// </summary>
        public string Name { get; set; }

        public IReference AvailablePackagesMountTarget { get; }

        public MountOptions DefaultMountOptions { get; set; }

        /// <summary>
        /// True can be a potential security issue, allowing users to augment/override program data.
        /// Default: !LionFireEnvironment.IsHardenedEnvironment
        /// </summary>
        public bool AddExistingOverlaySources { get; set; } 

        #region Subpaths

        // TODO: Make AvailableSubPath IReference, with typical usage a RelativeVobReference?
        public string AvailableSubPath { get; set; } = "available";

        //public bool MountEnabledUnderEnabledFolder { get; set; } = false;
        //public string EnabledSubPath { get; set; } = "enabled";

        /// <summary>
        /// Can be relative or absolute path
        /// </summary>
        public string DataLocation { get; set; } = "data";

        #endregion


        #region Derived

        /// <summary>
        /// If false, there will be no overlayed data at the PackageManger's data location
        /// </summary>
        public bool IsDataEnabled => DataLocation != null;

        #endregion

        #region Whitelist / Blacklist

        public ICollection<string> StoreNameWhitelist { get; set; }
        public ICollection<string> StoreNameBlacklist { get; set; }

        #endregion

        #region FUTURE: Required trust level or other conditions (e.g. PackageInfo.json) for available overlays

        //services.AddOverlayStack("base", new OverlayManagerOptions { StoreNameWhitelist = new List<string> { StoreNames.AppDir } })
        //public int TrustLevel { get; set; }

        #endregion

        #region Construction

        public OverlayStackOptions() { AddExistingOverlaySources = !LionFireEnvironment.IsHardenedEnvironment; }
        public OverlayStackOptions(IReference availablePackagesMountTarget) : this() { AvailablePackagesMountTarget = availablePackagesMountTarget; }

        #endregion

        #region (Static) Defaults

        public static OverlayStackOptions Default { get; } = new OverlayStackOptions
        {
            DefaultMountOptions = MountOptions.DefaultRead,
        };

        #endregion
    }
}

