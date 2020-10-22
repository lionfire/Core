
using LionFire.Logging;
using System;
using System.Threading.Tasks;

namespace LionFire.ErrorReporting
{
    public abstract class ErrorReporterBase : IErrorReporter
    {
        private Task AppStartLogFailureErrorReportTask { get; }

        public ErrorReporterBase(IServiceProvider serviceProvider)
        {
            var appStartLogger = (IAppStartLogger)serviceProvider.GetService(typeof(IAppStartLogger));
            if(appStartLogger != null)
            {
                AppStartLogFailureErrorReportTask = ReportAppStartLoggerFailure(appStartLogger);
            }
        }

        Task ReportAppStartLoggerFailure(IAppStartLogger appStartLogger)
        {
            switch (appStartLogger.LogSucceeded)
            {
                case true:
                    return Task.CompletedTask;
                case false:
                    return ReportError(appStartLogger.LogError ?? "Unspecified AppStartLogger error.");
                case null:
                default:
                    return ReportError("AppStartLogger did not run.");
            }
        }

        public abstract Task ReportError(object error);
        public abstract Task HandleException(ExceptionEventArgs ex);
    }
}
