using LionFire.Threading;
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
        T PushTab<T>(string tabName = null)
            where T : class;

        void ShowBackgroundTab(string tabKey);
        T ShowControl<T>(string tabName = null) where T : class;
        T ShowModalControl<T>(string tabName = null) where T : class;
        T ShowBackgroundControl<T>(string tabName = null) where T : class;

        //bool ContainsBackground(string tabName);

        void Close();
        bool CloseTab();
        bool DoCloseTab();
        bool IsActive { get; }
        bool HasTabs { get; }
        object TopControl { get; }

        event Action<bool> TopmostChanged;

        event Action CurrentTabNameChanged;

        bool HideModalControl(string controlName);
        void BringToFront();

        void HideModalControl<T>() where T : class;


        //IDispatcher CurrentDispatcher { get; }
        IDocumentTab CurrentDocumentTab { get; }
        object CurrentWindow { get; }
    }
}
