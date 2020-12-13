using System;

namespace LionFire.UI.Conventions
{
    [Flags]
    public enum ViewLayer
    {
        Background = 1 << 1,
        Content = 1 << 2,
        HUD = 1 << 3,
        Overlay = 1 << 4,
    }

    public static class ViewLayerExtensions
    {
        public static ViewLayer GetLayer(this UIReference viewReference)
            => (ViewLayer)viewReference.Layer;

        public static void SetLayer(this UIReference viewReference, ViewLayer shellLayer)
            => viewReference.Layer = (int)shellLayer;
    }

}
