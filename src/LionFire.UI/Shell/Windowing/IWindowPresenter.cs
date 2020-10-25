using System;

namespace LionFire.UI
{

    //public interface INamedViewsPresenter
    //{
    //    /// <summary>
    //    /// Show existing view, referenced by its name
    //    /// </summary>
    //    /// <param name="viewName"></param>
    //    void ShowView(string viewName);

    //}

    public interface ITabbedPresenter
    {
        bool TabsVisible { get; set; }
        // Dock TabsLocation {get;set;}
    }

    public interface IHasBackgroundPresenter
    {
        IPresenter BackgroundPresenter { get; }
    }

    //public class WindowPresenter: IWindowPresenter, ITabbedShellPresenter, ISingleViewPresenter, IHasBackgroundPresenter, INamedViewsPresenter, IHasMenuButton
    //{
    //}

    public interface IHasMenuButton
    {
        bool ShowMenuButton { get; set; }

    }

    public interface IWindowPresenterEx
    {
        bool IsActive { get; }

    }

    /// <summary>
    /// Controls the contents of a window
    /// </summary>
    public interface IOldPresenter : IPresenter
    {
        // TODO: Reduce this to INavigator?
                
        T PushTab<T>(string tabName = null)
            where T : class;

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
