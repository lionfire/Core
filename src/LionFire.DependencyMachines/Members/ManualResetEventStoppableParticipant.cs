using Nito.AsyncEx;
using System.Diagnostics;

namespace LionFire.DependencyMachines
{
    public class ManualResetEventStoppableParticipant : Participant<ManualResetEventStoppableParticipant>
    {
        public AsyncManualResetEvent StopResetEvent { get; }

        public ManualResetEventStoppableParticipant(string key)
        {
            this.Key = key;
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
