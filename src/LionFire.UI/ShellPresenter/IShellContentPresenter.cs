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

    public interface IShellContentPresenter
    {
        bool ShowMenuButton { get; set; }
        string CurrentTabViewName { get; }

        void ShowTab(string tabKey);
        T PushTab<T>(string tabName = null)
            where T : class;

        bool IsActive { get; }
        void ShowBackgroundTab(string tabKey);
        T ShowControl<T>(string tabName = null) where T : class;
        T ShowModalControl<T>(string tabName = null) where T : class;
        T ShowBackgroundControl<T>(string tabName = null) where T : class;

        //bool ContainsBackground(string tabName);

        
        bool CloseTab();
        bool DoCloseTab();
        
        bool HasTabs { get; }
        object TopControl { get; }

        event Action CurrentTabNameChanged;

        bool HideModalControl(string controlName);
        
        void HideModalControl<T>() where T : class;


        //IDispatcher CurrentDispatcher { get; }
        IDocumentTab CurrentDocumentTab { get; }

        void Close();

    }
}
