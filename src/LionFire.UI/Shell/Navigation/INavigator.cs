#nullable enable
using System.Threading.Tasks;

namespace LionFire.Shell
{
    public interface INavigator
    {
        /// <summary>
        /// Navigate to the context
        /// </summary>
        /// <param name="context">Examples: A URI string for web browsers.  For WPF, it could be a custom UserControl.  If MVVM support is available, it can be a custom ViewModel type</param>
        /// <param name="viewName"></param>
        /// <param name="options"></param>
        Task Show(ViewReference context, string? viewName = null, ViewParameters? options = null);

        /// <summary>
        /// Close the view with the provided name, or the currently selected view if name is null.
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        bool Close(string? viewName = null); // MOVE to GetPresenter(viewPath).Close()

    }
}
