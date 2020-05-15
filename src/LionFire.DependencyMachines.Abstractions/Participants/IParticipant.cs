#nullable enable
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{

    public interface ITryStartable
    {
        Task<object?> TryStartAsync(CancellationToken cancellationToken);
    }
    public interface ITryStoppable
    {
        Task<object?> TryStopAsync(CancellationToken cancellationToken);
    }

    public interface IParticipant : IKeyable 
    {
        List<object>? Dependencies { get; set; }
        List<object>? After { get; set; }

        IEnumerable<IReadWriteHandle>? DependencyHandles { get;  }

        List<object>? Provides { get; set; }
        List<object>? Contributes { get; set; }
        List<object>? PrerequisiteFor { get; set; }

        ParticipantFlags Flags { get; }

        bool HasKey { get; }
    }

    public interface IParticipant<TConcrete> : IParticipant
        where TConcrete : IParticipant
    {
        Func<TConcrete, CancellationToken, Task<object?>>? StartTask { get; set; }
    }

}
