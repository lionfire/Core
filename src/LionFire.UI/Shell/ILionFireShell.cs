using LionFire.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Microsoft.Extensions.Hosting;
using LionFire.Execution;

namespace LionFire.Shell
{

    public interface INotifiesClosing
    {
        event Action Closing; // Make cancelable?
        event Action Closed;
    }

    /// <summary>
    /// Responsible for showing the primary user interface.
    /// Started/Stopped by ILionFireApp, (or with the main DI provider via AddHostedService<>() directly)
    /// </summary>
    public interface ILionFireShell : IHostedService
        , IKeyboardShell
        , IPresenterShell
        , IRecoverableErrorShell
        //, INotifiesClosing
        //, IStoppable
        // TODO: Closing/Closed event interface? 

    {
    }
}
