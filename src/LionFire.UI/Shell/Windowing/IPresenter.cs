using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public interface IUINode
    {
        object View { get; set; }
    }

    

    public interface INamedUINode : IUINode, IKeyed
    {

        #region Derived

        string Path { get; }

        #endregion
    }


    /// <summary>
    /// Can show child UI elements
    /// 
    /// - If AutoClose is true, will close itself after a child closes and no descendants have PreventsAutoClose set.
    /// - Active state
    /// - Show method can show a ViewModel, a View type, or create a ViewModel for some data (such as a URL)
    /// </summary>
    public interface IPresenter : INamedUINode
    {
        
        #region Parameters

        bool PreventAutoClose { get; }
        bool AutoClose { get; }

        #endregion

        #region State

        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        #endregion

        /// <summary>
        /// Navigate to the context
        /// </summary>
        /// <param name="context">Examples: A URI string for web browsers.  For WPF, it could be a custom UserControl.  If MVVM support is available, it can be a custom ViewModel type</param>
        /// <param name="viewName"></param>
        /// <param name="options"></param>
        Task Show(ViewInstantiation context, ViewParameters? options = null);
        
    }
}
