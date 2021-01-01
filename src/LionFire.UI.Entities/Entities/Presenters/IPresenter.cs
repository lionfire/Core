using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    /// <summary>
    /// Can show/hide keyed child UI elements
    /// 
    /// - If AutoClose is true, will close itself after a child closes and no descendants have PreventsAutoClose set.
    /// - Active state
    /// - Show method can show a ViewModel, a View type, or create a ViewModel for some data (such as a URL)
    /// 
    /// Potential interfaces:
    ///  - IAutoCloseable: if autoclosing is supported
    /// 
    /// </summary>
    public interface IPresenter : IUICollection
    {
        /// <summary>
        /// Brings IUIKeyed object with key 'key' to the front.  Invokes PresenterEvents.OnShowing to make it visible and activated if those features are supported.
        /// </summary>
        /// <param name="key"></param>
        void Show(string key); // REVIEW - intent of this is "MakeVisible" which is "BringToFront" in some frameworks.

        /// <summary>
        /// Also see:Remove() to close the child
        /// </summary>
        /// <param name="key"></param>
        void Hide(string key); // REVIEW - Hide may not apply in some cases, so not sure it should be in IPresenter.  Could be minimize, or unpin, etc.
    }
}
