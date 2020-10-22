using System;

namespace LionFire.Shell.Conventions
{
    [Flags]
    public enum ShellLayer
    {
        Background = 1 << 1,
        Content = 1 << 2,
        HUD = 1 << 3,
        Overlay = 1 << 4,
    }

    public static class ShellLayerExtensions
    {
        public static ShellLayer GetLayer(this ViewReference viewReference)
            => (ShellLayer)viewReference.Layer;

        public static void SetLayer(this ViewReference viewReference, ShellLayer shellLayer)
            => viewReference.Layer = (int)shellLayer;
    }
}
