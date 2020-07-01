using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace LionFire.Shell
{
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

        #region Exception Handling

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            l.LogCritical("DispatcherUnhandledException: " + e.Exception.ToString());
            //LionFireApp.Current.OnApplicationDispatcherException(sender, e);
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


        private static ILogger l = Log.Get();

    }
}
