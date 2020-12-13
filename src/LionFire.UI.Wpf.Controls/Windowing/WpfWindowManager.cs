using LionFire.Shell;
using LionFire.UI.Entities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.UI.Entities
{

    public class WpfWindowFactory : IWindowFactory //: UICollection<IWindow>, IWindowCollection
    {
        #region Dependencies

        public IOptionsMonitor<UIOptions> UIOptionsMonitor { get; }
        IOptionsMonitor<WindowingOptions> WindowingOptionsMonitor { get; }
        WindowingOptions WindowingOptions => WindowingOptionsMonitor.CurrentValue;

        #endregion

        #region Construction

        public WpfWindowFactory(IOptionsMonitor<UIOptions> uiOptionsMonitor, IOptionsMonitor<WindowingOptions> windowingOptionsMonitor)
        {
            //Key = "(Window Manager)";
            UIOptionsMonitor = uiOptionsMonitor;
            WindowingOptionsMonitor = windowingOptionsMonitor;
        }

        #endregion

        #region (Public) Methods

        public Task<IWindow> Create(string windowName, object context = null, IDictionary<string, object> settings = null)
        {
            var window = new WpfMultiplexedWindow();
            window.Restore();
            return Task.FromResult<IWindow>(window);
        }

        #endregion

    }

    //public interface IWindowVM
    //{
    //}

    //public class WpfWindowVM : IWindowVM
    //{
    //    ShellWindow ShellWindow { get; private set; }
    //    FullScreenShellWindow FullScreenShellWindow { get; private set; }
    //}
}
