using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Packages;

public class PackageProviderDefaults
{
    public static PackageProviderDefaults Default { get; set; } = new PackageProviderDefaults();

    /// <summary>
    /// True can be a potential security issue, allowing users to augment/override program data.
    /// Default: !LionFireEnvironment.IsHardenedEnvironment
    /// </summary>
    public bool IsAutoRegisterAvailablePackagesEnabled { get; set; } 

    public PackageProviderDefaults()
    {
        IsAutoRegisterAvailablePackagesEnabled = !LionFireEnvironment.IsHardenedEnvironment;
    }
}

