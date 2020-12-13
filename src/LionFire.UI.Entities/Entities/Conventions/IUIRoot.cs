using LionFire.Referencing;
using LionFire.Shell;
using LionFire.Structures;
using LionFire.UI;
using LionFire.UI.Windowing;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace LionFire.UI.Entities
{
    // TODO: make sure all things from the summary are actually implemented:
    /// OLD for root:
    /// Logic for the display and lifetime management of the primary interactive presentation layer.
    ///  - Shows the startup interface
    ///  - Provides Close() method, (cancelable by the Closing event)
    ///  - Handles close events initiated by the user interface 
    ///  - Handles stop events from IHostApplicationLifetime
    ///  - Handles stop events from IHostedService.StopAsync

    /// <summary>
    /// Root of the UI object hierarchy.  Basically just a IUICollection with a hardcoded and mostly unused key of "(root)".
    /// </summary>
    /// <remarks>
    /// Typically there is one singleton for the application.  If the application has a secondary UI for development or back door access, 
    /// there could be multiple IUIRoots -- perhaps registered as named singletons
    /// </remarks>
    public interface IUIRoot :  IUICollection, IActivated
    {
        #region Options

        UIOptions Options { get; }

        #endregion
        
    }
}
