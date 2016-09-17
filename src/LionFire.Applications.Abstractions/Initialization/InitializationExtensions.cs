using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.Applications
{
    public static class InitializationExtensions
    {

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        /// <param name="retryDelayMilliseconds">Wait this long before attempting to resolve the dependency again.</param>
        /// <param name="extraRetries">Normally, at least one ApplicationTask must successfully initialize after each attempt to initialize remaining ApplicationTasks.  Provide a number above zero to give that many extra attempts to resolve.  Only the first time an above-zero number is provided will it be respected.</param>
        public static void OnUnfulfilledDependency(this IAppTask task, string message, int retryDelayMilliseconds = 0, int extraRetries = 0)
        {
            // TODO
            //task.ServiceProvider.
            //ManualSingleton<IApplicationHost>.Instance?.OnUnfulfilledDependency(task, message);
        }
    }
}
