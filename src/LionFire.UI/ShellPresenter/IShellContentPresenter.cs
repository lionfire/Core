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

        void Show();
        void ShowTab(string tabKey);
        void ShowBackgroundTab(string tabKey);

        //bool ContainsBackground(string tabName);

        void Close();
        bool DoCloseTab();
        bool IsActive { get; }
        bool HasTabs { get; }
        object TopControl { get; }

        event Action<bool> TopmostChanged;

        event Action CurrentTabNameChanged;

        bool HideModalControl(string controlName);
        void BringToFront();
    }
}
