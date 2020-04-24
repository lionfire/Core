using Nito.AsyncEx;
using System.Diagnostics;

namespace LionFire.DependencyMachines
{
    public class ManualDependencyContributor : Contributor
    {
        public AsyncManualResetEvent StartResetEvent { get; }

        public ManualDependencyContributor(string name) : base(name)
        {
            StartResetEvent = new AsyncManualResetEvent();
            StartTask = async (_, cancellationToken) =>
            {
                Debug.WriteLine($"{this} waiting for StartResetEvent...");
                await StartResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                Debug.WriteLine($"{this} waiting for StartResetEvent...done.");
                return null;
            };
        }
    }

    public class StoppableManualDependencyContributor : Contributor
    {
        public AsyncManualResetEvent StopResetEvent { get; }

        public StoppableManualDependencyContributor(string name) : base(name)
        {
            StopResetEvent = new AsyncManualResetEvent();
            StopTask = async (_, cancellationToken) =>
            {
                Debug.WriteLine($"{this} waiting for StopResetEvent...");
                await StopResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                Debug.WriteLine($"{this} waiting for StopResetEvent...done.");
                return null;
            };
        }
    }
}
