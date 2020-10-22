using LionFire.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Shell
{
    public interface IWindowedPresenter
    {
        void Show();
        void BringToFront();
        object CurrentWindow { get; }
        event Action<bool> TopmostChanged;

        bool HasFullScreenShellWindow { get; }
        bool HasShellWindow { get; }
    }


    

    public interface ISingleViewShellPresenter
    {
    }

    public interface IDockPaneShellPresenter
    {
    }
}
