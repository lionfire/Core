#define TRACE_PROGRESSIVETASK
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    

    public abstract class ProgressiveJob : IHasRunTask, IHasDescription, IHasProgress, IHasProgressMessage, IJob, IExecutable
    {

        public bool IsCompleted { get { return progress.Value >= 1; } }
        public string Description { get; set; }

        public IObservable<double> Progress { get { return progress; } }
        private BehaviorSubject<double> progress = new BehaviorSubject<double>(double.NaN);

        public CancellationToken CancellationToken { get; set; }

        public Task RunTask { get; set; }

        public IObservable<string> ProgressMessage { get { return progressMessage; } }
        private BehaviorSubject<string> progressMessage = new BehaviorSubject<string>("Not yet started.");

        public void UpdateProgress(double progressFactor, string message = null)
        {
            // TODO: Log
#if TRACE_PROGRESSIVETASK
            Console.WriteLine(this.ToString() + $" {progressFactor*100.0}% {message}");
#endif
            progress.OnNext(progressFactor);
            if (message != null) { progressMessage.OnNext(message); }
        }

        public IBehaviorObservable<ExecutionState> State { get { return state; } }
        protected BehaviorObservable<ExecutionState> state = new BehaviorObservable<ExecutionState>(ExecutionState.Ready);

        public abstract Task Start();
    }
}
