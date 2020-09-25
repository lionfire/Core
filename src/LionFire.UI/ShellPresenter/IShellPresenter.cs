
using LionFire.UI;
using LionFire.UI.Windowing;
using System;

namespace LionFire.Shell
{
    public interface IShellPresenter 
    {
        ILionFireShell Shell { get; }
        IShellContentPresenter MainPresenter { get; }
        WindowSettings WindowSettings { get; set; }
        IServiceProvider ServiceProvider { get; }

        void Show(UIReference reference);

        void ShowStartupInterfaces();

        void Close();

        ShellOptions Options { get; }
    }
}
