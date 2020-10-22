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

    public interface IParticipant : IKeyable 
    {
        List<object>? Dependencies { get; set; }
        List<object>? After { get; set; }

        IEnumerable<IReadWriteHandle>? DependencyHandles { get;  }

        List<object>? Provides { get; set; }
        List<object>? Contributes { get; set; }

        /// <summary>
        /// Execute in the same stage as these.  Will fail if there is more than one and they are in different stages
        /// </summary>
        //List<object>? ExecuteWith { get; set; } // FUTURE

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
