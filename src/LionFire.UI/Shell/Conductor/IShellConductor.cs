
using LionFire.UI;
using LionFire.UI.Windowing;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Shell
{

    // TODO: make sure all things from the summary are actually implemented:
    /// <summary>
    /// Logic for the display and lifetime management of the primary interactive presentation layer.
    ///  - Shows the startup interface
    ///  - Provides Close() method, (cancelable by the Closing event)
    ///  - Handles close events initiated by the user interface 
    ///  - Handles stop events from IHostApplicationLifetime
    ///  - Handles stop events from IHostedService.StopAsync
    /// </summary>
    public interface IShellConductor : IHostedService, IConductor
    {

        #region Children

        IPresenter MainPresenter { get; }

        #endregion

        #region Options

        WindowSettings WindowSettings { get; }

        ShellOptions Options { get; }

        #endregion

    }
}
