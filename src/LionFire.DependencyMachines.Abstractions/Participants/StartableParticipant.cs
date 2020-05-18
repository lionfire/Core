#nullable enable
using LionFire.Ontology;
using LionFire.Persistence;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    public sealed class StartableParticipant : StartableParticipant<StartableParticipant>, IHas<IServiceProvider>
    {
        public StartableParticipant(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            //Key = key; // ?? Guid.NewGuid().ToString();
        }

        public IServiceProvider ServiceProvider { get; }
        IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider;
    }

    public abstract class StartableParticipant<TConcrete> : IParticipant<TConcrete>, ITryStartable
        where TConcrete : StartableParticipant<TConcrete>
    {
        #region Key

        [SetOnce]
        public string? Key
        {
            get => key ?? DefaultKey;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException();
                key = value;
            }
        }
        private string? key;
        public bool HasKey => key != null;
        public virtual string? DefaultKey => this.Provides?.SingleOrDefault()?.ToDependencyKey() ?? $"{{{this.GetType().Name} {Guid.NewGuid()}}}";

        #endregion

        public void Freeze() => this.key ??= DefaultKey; // TODO - invoke this, or remove it if not needed

        public override string ToString() => Key ?? "{Participant Key=null}";

        public ParticipantFlags Flags { get; set; }

        public List<object>? After
        {
            get => after;
            set => after = value;
        }
        private List<object>? after;

        #region Dependencies

        public List<object>? Dependencies
        {
            get => dependencies;
            set => dependencies = value;
        }
        private List<object>? dependencies;

        public IEnumerable<IReadWriteHandle>? DependencyHandles { get; set; }
        public IEnumerable<IReadWriteHandle> UnsatisfiedDependencies => DependencyHandles.Where(h => !h.HasValue);
        public IEnumerable<IReadWriteHandle> SatisfiedDependencies => DependencyHandles.Where(h => h.HasValue);

        #endregion

        #region Provides

        public List<object>? Provides
        {
            get => provides;
            set => provides = value;
        }
        private List<object>? provides;

        #endregion

        #region Contributes

        public List<object>? Contributes { get; set; }

        #endregion
        public List<object>? PrerequisiteFor { get; set; }


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
        public Func<TConcrete, CancellationToken, object?> StartFunc
        {
            set
            {
                StartTask = (concrete, ct) =>
                {
                    return Task.FromResult(value(concrete, ct));
                };
            }
        }

        #endregion

        public bool RerunOnChanges { get; set; }

        public Task<object?> TryStartAsync(CancellationToken cancellationToken)
        {
            if (StartTask != null) return StartTask.Invoke((TConcrete)this, cancellationToken);
            return Task.FromResult<object?>(null);
        }



    }

}
