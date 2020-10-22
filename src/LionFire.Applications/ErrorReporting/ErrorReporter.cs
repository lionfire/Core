#if NBug
using NBug;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ErrorReporting
{

    public class ErrorReporter : IErrorReporter
    {

#if TOPORT
        #region Bug Reporting

        #region Configuration

        // Only for .NET 4+
        public virtual bool LogFirstChanceExceptions => true;

        #endregion

        /// <summary>
        /// Call once at startup
        /// </summary>
        private void TryEnableBugReporter()
        {
            if (!IsBugReporterEnabled) return;
            try
            {
                SplashMessage = "Enabling BugReporter";
                if (_bugReporter == null)
                {
                    if (IsDevMode)
                    {
                        _bugReporter = new NullBugReporter();
                    }
                    else
                    {
                        _bugReporter
#if NBug
 = new NBugReporter();
#else
 = new NullBugReporter();
#endif
                    }
                }

                BugReporter.IsEnabled = true;
            }
            finally
            {
                SplashMessage = "BugReporter enabled";
            }
        }
        private IBugReporter BugReporter
        {
            get
            {
                return _bugReporter;
            }
        }
        private IBugReporter _bugReporter;

#if !UNITY

        public bool OnApplicationDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            bool result = false;

            if (BugReporter != null) { result = BugReporter.OnApplicationDispatcherException(sender, e); }

            // REVIEW - RECENTCHANGE
            if (!result)
            {
                result = Shell.AskUserToContinueOnException(e.Exception);
            }
            e.Handled = result;
            return result;
        }

#endif

        #endregion

#endif
        public Task HandleException(ExceptionEventArgs ex) => throw new NotImplementedException();
        public Task ReportError(object error) => throw new NotImplementedException();
    }
}
