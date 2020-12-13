using LionFire.Collections;

namespace LionFire.UI.Entities.Conventions
{
    public static class UIConventions
    {
        public static class Keys
        {

            public static string MainWindow { get; set; } = "MainWindow";
            public static string WindowManager { get; set; } = "(windows)";
            public static string Layers { get; set; } = "(layers)";
            //public static string MainPresenter { get; set; } = "Main";
            public static string TopLayer { get; set; } = "^Top";
        }


        public static IWindow MainWindow(this IUIRoot root) => (IWindow)root.QuerySubPath(Keys.WindowManager, Keys.MainWindow);
        //public static IUIKeyed MainPresenter(this IUIRoot root) => (IUIKeyed)root.QuerySubPath(Keys.WindowManager, Keys.MainWindow, Keys.MainPresenter);


                        //new UIInstantiation(typeof(WpfLayers), "(windows)/MainWindow/(layers)"),
                        //new UIInstantiation(typeof(WpfLayer), "(windows)/MainWindow/(layers)/^Top"), // ENH: (layers) registers as collection decorator for keys starting with ^
                        //new UIInstantiation(typeof(WpfLayer), "(windows)/MainWindow/(layers)/^Tabs"),
                        //new UIInstantiation(typeof(WpfTabs), "(windows)/MainWindow/(layers)/^Tabs/(tabs)"),
                        //new UIInstantiation(typeof(WpfTextBox), "(windows)/MainWindow/(layers)/^Tabs/(tabs)/HelloWorld"),

    }
}
