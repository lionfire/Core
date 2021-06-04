namespace LionFire.UI
{
    public static class LayerConventions
    {
        public static LayerDefinition Background = new LayerDefinition
        {
            Depth = -100,
            Key = "Background",
        };

        public static LayerDefinition Content = new LayerDefinition
        {
            Depth = 1,
            Key = "Content",
        };
    }

}

