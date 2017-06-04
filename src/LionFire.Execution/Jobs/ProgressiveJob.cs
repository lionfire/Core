#define TRACE_PROGRESSIVETASK
using LionFire.Execution.Jobs;
using LionFire.Extensions.Logging;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    public abstract class ProgressiveJob : JobBase, IHasRunTask, IHasDescription, IHasProgress, IHasProgressMessage, INotifyPropertyChanged
    {

        public ProgressiveJob()
        {
            logger = this.GetLogger();
        }

        public override bool IsCompleted { get { return progress >= 1; } }
        public string Description { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        #region Progress

        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress == value) return;
                var oldIsCompleted = IsCompleted;
                progress = value;
                OnPropertyChanged(nameof(Progress));
                if (IsCompleted != oldIsCompleted) OnPropertyChanged(nameof(IsCompleted));
            }
        }
        private double progress = double.NaN;

        #endregion

        #region ProgressMessage

        public string ProgressMessage
        {
            get { return progressMessage; }
            set
            {
                if (progressMessage == value) return;
                progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }
        private string progressMessage;

        #endregion

        public void UpdateProgress(double progressFactor, string message = null, LogLevel? logLevel = null)
        {
            if (CancellationToken != null && CancellationToken.Value.IsCancellationRequested) throw new OperationCanceledException(CancellationToken.Value);

#if TRACE_PROGRESSIVETASK
            if(!logLevel.HasValue) logLevel = LogLevel.Trace;
#endif
            if (logLevel.HasValue && logLevel.Value != LogLevel.Disabled)
            {
                var logMessage = this.ToString() + $" {progressFactor * 100.0}% {message}";
                logger.Log(logLevel.Value, logMessage);
            }
            Progress = progressFactor;
            if (message != null) { ProgressMessage = message; }
        }

        protected ILogger logger;
    }

    public static class ILoggerExtensions
    {
        public static void Log(this ILogger logger, LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Disabled:
                    break;
                case LogLevel.Fatal:
                    break;
                case LogLevel.Critical:
                    break;
                case LogLevel.Error:
                    break;
                case LogLevel.Warning:
                    //case LogLevel.Warn:
                    logger.LogWarning(message);
                    break;
                case LogLevel.MajorMessage:
                    break;
                case LogLevel.Message:
                    break;
                case LogLevel.MinorMessage:
                    break;
                case LogLevel.Info:
                    break;
                case LogLevel.Verbose:
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message);
                    break;
                case LogLevel.Trace:
                    logger.LogTrace(message);
                    break;
                case LogLevel.Default:
                    break;
                case LogLevel.Step:
                    break;
                case LogLevel.All:
                    break;
                case LogLevel.Unspecified:
                    break;
                default:
                    break;
            }
        }
    }
}
