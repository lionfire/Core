using LionFire.Applications;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Microsoft.Extensions.Hosting;
using LionFire.Execution;

namespace LionFire.Shell
{
    // OLD.  See IShellLifetime and IShellConductor

    ///// <summary>
    ///// Responsible for showing the primary user interface.
    ///// Started/Stopped by ILionFireApp, (or with the main DI provider via AddHostedService<>() directly)
    ///// </summary>
    //public interface ILionFireShell 
    //    //INotifyClosing
    ////, INotifiesClosing
    ////, IRecoverableErrorShell
    ////, IStoppable
    //// TODO: Closing/Closed event interface? 
    //{
    //    //ShellOptions ShellOptions { get; }

    //    void Close();
    //}
}
