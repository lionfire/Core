#if NOESIS
using System;
//using AppUpdate;
//using AppUpdate.Common;
//using System.Windows.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using LionFire.UI;
using Noesis;
using LionFire.UI.Wpf;
using LionFire.Shell.Wpf;
using LionFire.Applications.Hosting;
using LionFire.Dispatching;
using LionFire.Threading;
#if UNITY
using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper;
#endif
using LionFire.Dependencies;

namespace LionFire.Shell
{
    // REVIEW - For more clear DLL responsibilities, move dispatcher/shell support to something like LionFire.UI.Unity.Noesis.

    public class NoesisUnityShell : WpfShellBase<NoesisUnityShellPresenter>
    {
#region (Static) Instance

        public static NoesisUnityShell Instance => DependencyContext.Default.GetService<NoesisUnityShell>();

#endregion

        public override IDispatcher Dispatcher => dispatcher;
        private static IDispatcher dispatcher = new UnityThreadDispatcherWrapper();

        public NoesisUnityShell(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, IOptionsMonitor<StartupInterfaceOptions> interfaceOptionsMonitor, IViewLocator viewLocator, NoesisUnityShellPresenter shellPresenter, IOptionsMonitor<ShellOptions> shellOptionsMonitor)
        : base(serviceProvider, hostApplicationLifetime, interfaceOptionsMonitor, viewLocator, shellPresenter, shellOptionsMonitor)
        {
#region Derived

            //WpfDispatcherAdapter = new WpfDispatcherAdapter(Application);

#endregion
        }
    }
}
#endif