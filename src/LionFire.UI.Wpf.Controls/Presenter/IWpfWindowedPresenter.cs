#if WPF
using System.Windows;

namespace LionFire.Shell
{
    public interface IWpfWindowedPresenter : IWindowedPresenter
    {
        FullScreenShellWindow FullScreenShellWindow { get; }
        ShellWindow ShellWindow { get; }
        bool Topmost { get; }
    }

    public interface IWpfShellContentPresenter : IOldPresenter, IWpfWindowedPresenter
    {
        
    }

}
#endif
