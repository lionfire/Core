#nullable enable
using LionFire.Persistence;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    public sealed class Participant : Participant<Participant>
    {
        public override string Key { get; }

        public Participant(IServiceProvider serviceProvider, string? key = null)
        {
            ServiceProvider = serviceProvider;
            Key = key; // ?? Guid.NewGuid().ToString();
        }

        public IServiceProvider ServiceProvider { get; }
    }

    public abstract class Participant<TConcrete> : IParticipant, IHostedService
        where TConcrete : Participant<TConcrete>
    {
        public abstract string Key { get; }

        public ParticipantFlags Flags { get; set; }

        #region Dependencies

        public IEnumerable<object> Dependencies
        {
            get => dependencies ?? Enumerable.Empty<object>();
            set => dependencies = value;
        }
        private IEnumerable<object> dependencies;

        public IEnumerable<IReadWriteHandle> DependencyHandles => dependencyHandles ?? Enumerable.Empty<IReadWriteHandle>();
        private List<IReadWriteHandle> dependencyHandles;

        public IEnumerable<IReadWriteHandle> UnsatisfiedDependencies => DependencyHandles.Where(h => !h.HasValue);
        public IEnumerable<IReadWriteHandle> SatisfiedDependencies => DependencyHandles.Where(h => h.HasValue);

        #endregion

        #region Provides

        public IEnumerable<object> Provides
        {
            get => provides ?? Enumerable.Empty<object>();
            set => provides = value;
        }
        private IEnumerable<object> provides;

        #endregion

        #region Contributes

        IEnumerable<object> IParticipant.Contributes => contributes ?? Enumerable.Empty<object>();
        public List<object>? Contributes
        {
            get => contributes;
            set => contributes = value;
        }
        private List<object>? contributes;

        #endregion


        #region StartTask

        [SetOnce]
        public Func<TConcrete, CancellationToken, Task<object?>>? StartTask
        {
            get => startTask;
            set
            {
                if (startTask == value) return;
                if (startTask != default) throw new AlreadySetException();
                startTask = value;
            }
        }
        private Func<TConcrete, CancellationToken, Task<object?>>? startTask;

        public Action StartAction
        {
            set
            {
                StartTask = (concrete, ct) =>
                {
                    value();
                    return Task.FromResult<object?>(null);
                };
            }
        }
        public Func<object?> StartFunc
        {
            set
            {
                StartTask = (concrete, ct) =>
                {
                    return Task.FromResult(value());
                };
            }
        }

        #endregion

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
        public Func<object?> StopFunc
        {
            set
            {
                StopTask = (concrete, ct) =>
                {
                    return Task.FromResult(value());
                };
            }
        }

        #endregion

        public bool RerunOnChanges { get; set; }

        public Task<object?> StartAsync(CancellationToken cancellationToken)
        {
            if (StartTask != null) return StartTask.Invoke((TConcrete)this, cancellationToken);
            return Task.FromResult<object?>(null);
        }
        public Task<object?> StopAsync(CancellationToken cancellationToken)
        {
            if (StopTask != null) return StopTask.Invoke((TConcrete)this, cancellationToken);
            return Task.FromResult<object?>(null);
        }

        public override string ToString() => Key;

    }

    public static class ParticipantExtensions
    {
        public static T Contributes<T>(this T participant, string stage)
            where T : Participant<T>
        {
            participant.Contributes ??= new List<object>();
            participant.Contributes.Add(stage);
            return participant;
        }
    }

}
