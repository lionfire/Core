using LionFire.Collections;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI
{
    
    

    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public interface IWindowManager : IHierarchical<INamedUINode>
    {
        
        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> Windows { get; }
        
        Task<IPresenter> CreateWindow(string windowName, object context = null, IDictionary<string, object> settings = null);

        #region From Caliburn.Micro

        //        /// <summary>
        //        /// Shows a modal dialog for the specified model.
        //        /// </summary>
        //        /// <param name="rootModel">The root model.</param>
        //        /// <param name="context">The context.</param>
        //        /// <param name="settings">The optional dialog settings.</param>
        //        /// <returns>The dialog result.</returns>
        //        Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);

        //        /// <summary>
        //        /// Shows a non-modal window for the specified model.
        //        /// </summary>
        //        /// <param name="rootModel">The root model.</param>
        //        /// <param name="context">The context.</param>
        //        /// <param name="settings">The optional window settings.</param>
        //        Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);

        //        /// <summary>
        //        /// Shows a popup at the current mouse position.
        //        /// </summary>
        //        /// <param name="rootModel">The root model.</param>
        //        /// <param name="context">The view context.</param>
        //        /// <param name="settings">The optional popup settings.</param>
        //        Task ShowPopupAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);

        #endregion

    }

    public static class WindowManagerExtensions
    {
        public static IPresenter QueryWindow(this IWindowManager windowManager, string windowName) => windowManager.Windows[windowName];
    }
}
