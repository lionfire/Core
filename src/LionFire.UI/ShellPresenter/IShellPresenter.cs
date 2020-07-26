
using LionFire.UI;
using LionFire.UI.Windowing;

namespace LionFire.Shell
{
    public interface IShellPresenter 
    {
        IShellContentPresenter MainPresenter { get; }
        WindowSettings WindowSettings { get; set; }

        void Show(UIReference reference);

        void ShowStartupInterfaces();

        void Close();
    }
}
