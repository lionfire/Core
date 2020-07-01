using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using LionFire.Threading;

namespace LionFire.Shell
{
    public class WpfDispatcherAdapter : IDispatcher
    {

        readonly Dispatcher dispatcher;

        public WpfDispatcherAdapter(Application application)
        {
            dispatcher = application.Dispatcher;

            dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) => throw new NotImplementedException();

        public WpfDispatcherAdapter(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public bool IsInvokeRequired => throw new NotImplementedException();

        public event EventHandler<Threading.DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action) => dispatcher.BeginInvoke(action).Task;
        public async Task<object> BeginInvoke(Func<object> func)
        {
            var r = dispatcher.BeginInvoke(func);
            await r.Task;
            return r.Result;
        }

        public void Invoke(Action action) => dispatcher.Invoke(action);

        public object Invoke(Func<object> func) => dispatcher.Invoke(func);

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
    }
}
