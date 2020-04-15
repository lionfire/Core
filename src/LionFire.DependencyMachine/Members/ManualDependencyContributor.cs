using Nito.AsyncEx;

namespace LionFire.DependencyMachine
{
    public class ManualDependencyContributor : DependencyContributor
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
                    await StartResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return null;
                };
            }
            if (isStopManual)
            {
                StopResetEvent = new AsyncManualResetEvent();
                StopAction = async (_, cancellationToken) =>
                {
                    await StartResetEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return null;
                };
            }
        }
    }
}
