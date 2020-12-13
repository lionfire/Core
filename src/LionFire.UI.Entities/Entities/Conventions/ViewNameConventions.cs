namespace LionFire.UI
{
    public class LayerDefinition
    {
        public int Depth { get; set; }
        public string Key { get; set; }
    }
    public class LayerConventions
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
    public class ViewNameConventions
    {
        public const string MainWindow = "MainWindow";
        public const string Layers = "(Layers)";
        /// <summary>
        /// Used as the tab name when DefaultViewType is used.
        /// </summary>
        public const string DefaultViewName = "__Default";
        public const string LaunchTabName = "__Launch";
    }

}

