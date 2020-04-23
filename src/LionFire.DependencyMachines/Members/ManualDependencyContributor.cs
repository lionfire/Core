using Nito.AsyncEx;
using System.Diagnostics;

namespace LionFire.DependencyMachines
{
    public class ManualDependencyContributor : Contributor
    {
        public AsyncManualResetEvent StartResetEvent { get; } 
        public AsyncManualResetEvent StopResetEvent { get; } 

        public ManualDependencyContributor(string name, bool isStartManual = true, bool isStopManual = false) : base(name)
        {

            if (isStartManual)
            {
                StartResetEvent = new AsyncManualResetEvent();
                StartAction = async (_, cancellationToken) =>
                {
                    Debug.WriteLine($"{this} waiting for StartResetEvent...");
                    await StartResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                    Debug.WriteLine($"{this} waiting for StartResetEvent...done.");
                    return null;
                };
            }
            if (isStopManual)
            {
                StopResetEvent = new AsyncManualResetEvent();
                StopAction = async (_, cancellationToken) =>
                {
                    Debug.WriteLine($"{this} waiting for StopResetEvent...");
                    await StartResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                    Debug.WriteLine($"{this} waiting for StopResetEvent...done.");
                    return null;
                };
            }
        }
    }
}
