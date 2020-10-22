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
using LionFire.ErrorReporting;
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Shell
{
    public static class WpfApplicationExtensions
    {
        public static void TryInitErrorReporter(this Application application, IServiceProvider serviceProvider)
        {
            var ErrorReporter = serviceProvider.GetService<IErrorReporter>();
            if (ErrorReporter != null)
            {
                application.DispatcherUnhandledException +=
                    (object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
                        =>
                    {
                        var args = new ExceptionEventArgs(e.Exception, e.Dispatcher) { Handled = e.Handled };
                        ErrorReporter.HandleException(args).Wait();
                        e.Handled = args.Handled;
                    };
            }
        }
    }
}
