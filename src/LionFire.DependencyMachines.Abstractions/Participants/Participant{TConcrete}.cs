#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    public abstract class Participant<TConcrete> : StartableParticipant<TConcrete>, ITryStoppable
        where TConcrete : Participant<TConcrete>
    {
        #region StopTask

        [SetOnce]
        public Func<TConcrete, CancellationToken, Task<object?>>? StopTask
        {
            get => stopTask;
            set
            {
                if (stopTask == value) return;
                if (stopTask != default) throw new AlreadySetException();
                stopTask = value;
            }
        }
        private Func<TConcrete, CancellationToken, Task<object?>>? stopTask;

        public Action StopAction
        {
            set
            {
                StopTask = (concrete, ct) =>
                {
                    value();
                    return Task.FromResult<object?>(null);
                };
            }
        }
        public Func<TConcrete, CancellationToken, object?> StopFunc
        {
            set
            {
                StopTask = (concrete, ct) =>
                {
                    return Task.FromResult(value(concrete, ct));
                };
            }
        }

        #endregion
        public Task<object?> TryStopAsync(CancellationToken cancellationToken)
        {
            if (StopTask != null) return StopTask.Invoke((TConcrete)this, cancellationToken);
            return Task.FromResult<object?>(null);
        }
    }

}
