using LionFire.Applications.Hosting;
using LionFire.CommandLine.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
    public static class CommandLineAppHostExtensions
    {
        public static IAppHost AddCommandLineDispatcher(this IAppHost host, string[] args = null, Type dispatcherType = null, object dispatcher = null, object context = null, CliDispatcherOptions options = null)
        {
#if NET462
            if (args == null)
            {
                args = Environment.GetCommandLineArgs();
            }
#endif

            var task = new CommandLineDispatchTask()
            {
                Args = args,
                DispatcherType = dispatcherType,
                DispatcherObject = dispatcher,
                Options = options,
                Context = context,
            };

            host.Add(task);

            return host;
        }

        public static IAppHost AddCommandLineDispatcher<T>(this IAppHost host, string[] args = null, object context = null, CliDispatcherOptions options = null)
        {
#if NET462
            if (args == null)
            {
                args = Environment.GetCommandLineArgs();
            }
#endif

            var task = new CommandLineDispatchTask()
            {
                Args = args,
                DispatcherType = typeof(T),
                Options = options,
                Context = context,
            };

            host.Add(task);

            return host;
        }
    }
}
