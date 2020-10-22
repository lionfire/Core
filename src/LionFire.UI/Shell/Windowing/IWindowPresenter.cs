using LionFire.Structures;
using System;

namespace LionFire.Shell
{
    public interface INamedViewsPresenter
    {
        /// <summary>
        /// Show existing view, referenced by its name
        /// </summary>
        /// <param name="viewName"></param>
        void ShowView(string viewName);

    }

    public interface IPresenterContainer
    {
        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> Presenters { get; }
    }

    public interface IPresenter : INavigator, IKeyed
    {
        string Path { get; }
        bool KeepsApplicationAlive { get; }

        bool IsActive { get; }
    }

    public interface ISingleViewPresenter
    {
        string CurrentViewName { get; }
    }

    public interface ITabbedPresenter
    {
    }

    public interface IHasBackgroundPresenter
    {
        IWindowPresenter BackgroundPresenter { get; }
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
    public interface IWindowPresenter : INavigator
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
