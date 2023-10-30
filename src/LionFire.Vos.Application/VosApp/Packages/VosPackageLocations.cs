using System;

namespace LionFire.Vos.VosApp;

public static class VosPackageLocations
{
    /// <summary>
    /// Environment key
    /// </summary>
    public static string Packages = "packages";

    /// <summary>
    /// Default: $"$package/{packageActivatorName}"
    /// </summary>
    //public static Func<string, string> GetPackageProviderPath { get; set; } = o => $"/${Packages}/{o}";
    public static Func<string, string> GetPackageProviderPath { get; set; } = o => $"${Packages}/{o}";

}

