using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    //public class PWindow
    //{

    //}

    public interface IWindowFactory
    {
        // REVIEW - Parameters are from Caliburn Micro
        Task<IWindow> Create(string windowName, object context = null, IDictionary<string, object> settings = null);
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
