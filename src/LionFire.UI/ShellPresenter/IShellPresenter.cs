
using LionFire.UI;

namespace LionFire.Shell
{
    public interface IShellPresenter 
    {
        IShellContentPresenter MainPresenter { get; }

        void Show(UIReference reference);

        void ShowRootViews();

        void Close();
    }
}
