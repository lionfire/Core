using System.Threading;
//using AppUpdate;
//using AppUpdate.Common;
using System.Threading.Tasks;
using System.Diagnostics;
#if NOESIS
using Noesis;
#if UNITY
using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper;
#endif
#else
#endif
#if WPF
//using System.Windows.Documents;
//using System.Windows.Threading;
#endif
using Microsoft.Extensions.Hosting;

namespace LionFire.Shell
{
    //public interface IWpfShell : ILionFireShell // , IHostedService, IKeyboardShell
    //{
    //    Application Application { get; }
    //}

    ///// <summary>
    /////  - IDispatcher
    ///// </summary>
    //public abstract class ShellBase : ILionFireShell
    //{
    //}


    public class WpfDiagnostics : IHostedService
    {
        public SourceLevels DataBindingSourceLevel { get; set; }
        public Task StartAsync(CancellationToken cancellationToken)
        {
#if !NOESIS
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = DataBindingSourceLevel;
#endif
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
