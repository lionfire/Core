#if NOESIS
using Noesis;
#if UNITY
using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper;
#endif
#else
#endif
using Caliburn.Micro;
using LionFire.ErrorReporting;
using LionFire.Ontology;
using LionFire.Shell;
using LionFire.Threading;
using System;
using System.Windows;
using System.Windows.Media;

namespace LionFire.UI
{
    /// <summary>
    /// Responsible for:
    ///  - Creating LionFire IDispatcher from WPF Application Dispatcher
    ///  - Integrating Application UnhandledExceptions to IErrorReporter
    /// </summary>
    public class WpfRuntime : IHas<IDispatcher>, IUIPlatform
    {
        public Application Application { get; }

        public WpfDispatcherAdapter WpfDispatcherAdapter { get; }
        IDispatcher IHas<IDispatcher>.Object => WpfDispatcherAdapter;
        public IDispatcher Dispatcher => WpfDispatcherAdapter;

        public IEventAggregator EventAggregator => throw new NotImplementedException();

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
        public bool IsViewType(Type type) => typeof(FrameworkElement).IsAssignableFrom(type); // Potential alternatives: Visual, DependencyObject, DispatcherObject
        public bool IsPlatformType(Type type) => typeof(System.Windows.Threading.DispatcherObject).IsAssignableFrom(type);
    }
}
