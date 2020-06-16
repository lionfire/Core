using System;
using System.Windows;
using Microsoft.Extensions.Logging;
using LionFire.Applications.ErrorReporting;

namespace LionFire.Shell
{
    // TOPORT - add to applications

    public class WpfErrorNotifier : IRecoverableErrorNotifier
    {
        /// <summary>
        /// Platform-default user exception handler
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool AskUserToContinueOnException(Exception ex)
        {
            //if (!isStarted)
            //{
            //    deferredAskUserToUpdate = true; // Defer until WPF Application is started
            //    // FUTURE: could defer only if we need to ask the user something
            //    return true;
            //}
            //deferredAskUserToUpdate = false; // reset

            l.Info("[FATAL] Asking user whether to continue after unhandled exception: " + ex.ToString());

#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            // TODO: Hide the UpdateManager reference in the Application framework?
            var dr = MessageBox.Show(
                    "An unhandled exception occurred.  Do you wish to continue?  If no, the application will be aborted.  Exception: " + Environment.NewLine + ex.ToString().Substring(0, Math.Min(500, ex.ToString().Length)),
                    "Unhandled exception",
                     MessageBoxButton.YesNo);

            if (dr == MessageBoxResult.Yes)
            {
                l.LogCritical("User chose to continue despite unhandled exception.");
                return true;
            }
            l.LogCritical("User chose to abort due to unhandled exception.");
            return false;
        }


        private static ILogger l = Log.Get();

    }
}
