using LionFire.Shell;

namespace LionFire.UI.Entities
{
    public class WpfWindowedWindow : WpfWindowBase<WindowedWindowView>
    {
        public WpfWindowedWindow()
        {
            Key = "(windowed)";
        }
    }
}
