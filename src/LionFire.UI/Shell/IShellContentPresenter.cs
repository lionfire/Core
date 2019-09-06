using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Shell
{
    public interface IShellContentPresenter
    {
        bool ShowMenuButton { get; set; }
        string CurrentTabViewName { get; }
        void ShowTab(string tabKey);
        void ShowBackgroundTab(string tabKey);

        //bool ContainsBackground(string tabName);

        void Close();
        bool DoCloseTab();
        bool IsActive { get; }

        event Action CurrentTabNameChanged;

        bool HideModalControl(string controlName);
    }
}
