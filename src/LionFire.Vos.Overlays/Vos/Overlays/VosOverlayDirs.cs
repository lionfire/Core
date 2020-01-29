using System;

namespace LionFire.Vos.Overlays
{
    public static class VosOverlayDirs
    {
        public static string Overlays = "$overlays";

        /// <summary>
        /// Default: $"$overlays/{overlayStackName}"
        /// </summary>
        public static Func<string, string> GetOverlayStackPath { get; set; } = o => $"{Overlays}/{o}";

    }
}

