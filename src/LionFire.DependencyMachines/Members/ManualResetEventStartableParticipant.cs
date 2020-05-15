using Nito.AsyncEx;
using System.Diagnostics;

namespace LionFire.DependencyMachines
{
    public class ManualResetEventStartableParticipant : Participant<ManualResetEventStartableParticipant>
    {
        public AsyncManualResetEvent StartResetEvent { get; }

        public ManualResetEventStartableParticipant(string key)
        {
            this.Key = key;
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
}
