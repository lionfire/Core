using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using LionFire.Threading;
using Microsoft.Extensions.Logging;

namespace LionFire.Shell
{
    public class WpfDispatcherAdapter : IDispatcher
    {

        public Dispatcher Dispatcher { get; }

        public WpfDispatcherAdapter(Application application)
        {
            Dispatcher = application.Dispatcher;

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) 
            => throw new NotImplementedException();

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            l.LogCritical("DispatcherUnhandledException: " + e.Exception.ToString());
            //LionFireApp.Current.OnApplicationDispatcherException(sender, e);
            UnhandledException?.Invoke(sender, new Threading.DispatcherUnhandledExceptionEventArgs(Dispatcher, e.Exception)); // TODO: make the parameters more like the WPF args
        }

        //public event EventHandler<System.Windows.Threading.DispatcherUnhandledExceptionEventArgs> UnhandledException;
        public event EventHandler<Threading.DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public WpfDispatcherAdapter(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public bool IsInvokeRequired => throw new NotImplementedException();


        public Task BeginInvoke(Action action) => Dispatcher.BeginInvoke(action).Task;
        public async Task<object> BeginInvoke(Func<object> func)
        {
            var r = Dispatcher.BeginInvoke(func);
            await r.Task;
            return r.Result;
        }

        public void Invoke(Action action) => Dispatcher.Invoke(action);

        public object Invoke(Func<object> func) => Dispatcher.Invoke(func);

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