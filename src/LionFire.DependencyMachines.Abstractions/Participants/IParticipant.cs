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
        List<object>? Dependencies { get; }

        IEnumerable<IReadWriteHandle> DependencyHandles { get;  }

        IEnumerable<object> Provides { get; }
        List<object>? Contributes { get; }

        ParticipantFlags Flags { get; }

    }

    public static class IParticipantExtensions
    {
        public static IEnumerable<IParticipant> GetParticipants(this object obj)
        {
            if (obj is IHas<IParticipant> hasParticipant)
            {
                yield return hasParticipant.Object;
            }
            if (obj is IHasMany<IParticipant> hasParticipants)
            {
                foreach (var p in hasParticipants.Objects) { yield return p; }
            }
        }
    }
}
