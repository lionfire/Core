using System.Collections.Generic;

namespace LionFire.Services
{
    // Not fully implemented.  Used with services.InitializeVob, services.InitializeRootVob

    public static class VosAppInitialization
    {
        public const string Stores = "Stores";
        public const string OverlaySources = "Overlay Sources";
        public const string Mounts = "Mounts";

        public static IEnumerable<string> StoreMounts { get { yield return Stores; yield return Mounts; } }

        public static IEnumerable<string> AsEnumerable(this string str) => new string[] { str };
    }
}
