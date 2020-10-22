#if NOESIS
using Noesis;
#if UNITY
using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper;
#endif
#else
#endif
using LionFire.ErrorReporting;
using LionFire.Ontology;
using LionFire.Threading;
using System;
using System.Windows;

namespace LionFire.Shell
{
    /// <summary>
    /// Responsible for:
    ///  - Creating LionFire IDispatcher from WPF Application Dispatcher
    ///  - Integrating Application UnhandledExceptions to IErrorReporter
    /// </summary>
    public class WpfRuntime : IHas<IDispatcher>
    {
        public Application Application { get; }

        public WpfDispatcherAdapter WpfDispatcherAdapter { get; }
        IDispatcher IHas<IDispatcher>.Object => WpfDispatcherAdapter;

        public WpfRuntime(IServiceProvider serviceProvider)
        {
            Application = Application.Current;
            if (Application == null)
            {
                Application = CreateApplication();
            }
            WpfDispatcherAdapter = new WpfDispatcherAdapter(Application);

            Application.TryInitErrorReporter(serviceProvider);
        }

        protected virtual Application CreateApplication() => new Application();
    }
}
