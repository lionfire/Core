using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using LionFire.Threading;
using Microsoft.Extensions.Logging;
using LionFire.ErrorReporting;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.UI
{
    public class WpfDispatcherAdapter : IDispatcher
    {

        #region Dependencies

        public Dispatcher Dispatcher { get; }
        public ILogger<WpfDispatcherAdapter> Logger { get; }
        public IErrorReporter ErrorReporter { get; }

        #endregion

        // TODO: register application.Dispatcher with IServiceProvider, and depend on it here?
        public WpfDispatcherAdapter(Dispatcher dispatcher, ILogger<WpfDispatcherAdapter> logger, IServiceProvider serviceProvider)
        {
            //Dispatcher = application.Dispatcher;
            Dispatcher = dispatcher;
            Logger = logger;
            ErrorReporter = serviceProvider.GetService<IErrorReporter>();

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        #region Event Handling

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(sender, new LionFire.Threading.DispatcherUnhandledExceptionEventArgs(Dispatcher, e.Exception));
            Logger.LogCritical(e.Exception, "Unhandled exception");
            ErrorReporter?.HandleException(e.Exception);
        }


#if UNUSED
        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            l.LogCritical("DispatcherUnhandledException: " + e.Exception.ToString());
            //LionFireApp.Current.OnApplicationDispatcherException(sender, e);
            UnhandledException?.Invoke(sender, new Threading.DispatcherUnhandledExceptionEventArgs(Dispatcher, e.Exception)); // TODO: make the parameters more like the WPF args
        }
#endif

        #endregion

        #region Events

        public event EventHandler<LionFire.Threading.DispatcherUnhandledExceptionEventArgs> UnhandledException; // LionFire event
        
        #endregion

        #region Queries

        public bool IsInvokeRequired => !Dispatcher.CheckAccess();

        #endregion

        #region Methods

        public Task BeginInvoke(Action action) => Dispatcher.BeginInvoke(action).Task;
        public async Task<object> BeginInvoke(Func<object> func)
        {
            var r = Dispatcher.BeginInvoke(func);
            await r.Task;
            return r.Result;
        }

        public void Invoke(Action action) => Dispatcher.Invoke(action);

        public object Invoke(Func<object> func) => Dispatcher.Invoke(func);

        #endregion

#if OLD
#region Dispatcher

        //public static Dispatcher DefaultDispatcher
        //{
        //    get
        //    {
        //        if (LionFireApp.Current != null) return LionFireApp.Current.Dispatcher;
        //        return Dispatcher.CurrentDispatcher;
        //    }
        //}
        //        public Dispatcher Dispatcher
        //        {
        //            get
        //            {
        //                if (dispatcher != null) return dispatcher;
        //#if PresentationFramework
        //                if(System.Windows.Application.Current != null && System.Windows.Application.Current.Dispatcher != null) return System.Windows.Application.Current.Dispatcher;
        //#endif
        //                return Dispatcher.CurrentDispatcher;
        //            }
        //            set { dispatcher = value; }
        //        }

#endregion

#region IDispatcher

        public void Invoke(Action action) { Dispatcher.Invoke(action); }

        object IDispatcher.Invoke(Func<object> f)
        {
            if (Application == null) { return f(); }
            return Application.Dispatcher.Invoke(f);
        }

        public void BeginInvoke(Action action)
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        }

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            l.LogCritical("DispatcherUnhandledException: " + e.Exception.ToString());
            var args = new LionFire.Threading.DispatcherUnhandledExceptionEventArgs(e.Dispatcher, e.Exception)
            {
                Handled = e.Handled,
            };
            UnhandledException?.Invoke(this, args);

            //if (!args.Handled)
            //{

            //}

            //LionFireApp.Current.OnApplicationDispatcherException(sender, e); // TOPORT 
        }

#endregion

#endif

        private static ILogger l = Log.Get();

    }
}

#if TOMERGE

using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace LionFire.Shell
{
    // UNUSED - merge into WpfDispatcherAdapter, maybe rename that class to WpfRuntime.

    /// <summary>
    /// Responsible for
    ///  - Application.Dispatcher
    ///
    /// </summary>
    public class WpfRuntime
    {
        // REVIEW - compare with Caliburn.Micro Invoke support

#region Construction

        public WpfRuntime()
        {
            Application.Dispatcher.UnhandledException += Current_DispatcherUnhandledException;
        }

#endregion

#region Derived

        public Dispatcher Dispatcher => Application.Dispatcher;

#endregion

#region Application

        public Application Application
        {
            get => application ??= ApplicationProvider();
            set => application = value;
        }
        private Application application;

        protected virtual Func<Application> ApplicationProvider
        {
            get
            {
                return () =>
                {
                    if (Application.Current != null) { return Application.Current; }
                    else { return new Application(); }
                };
            }
        }

#endregion

#region Invoke

        public void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }
        //public object Invoke(Func<object> f)
        //{
        //    if (Application == null) { return f(); }
        //    return Application.Dispatcher.Invoke(f);
        //}

        public void BeginInvoke(Action action)
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        }

#endregion
    }
}
#endif