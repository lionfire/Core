#nullable enable
using LionFire.Ontology;
using System;

namespace LionFire.DependencyMachines
{
    public sealed class Participant : Participant<Participant>, IHas<IServiceProvider>
    {
        public Participant(IServiceProvider? serviceProvider = null, string? key = null)
        {
            ServiceProvider = serviceProvider;
            Key = key; // ?? Guid.NewGuid().ToString();
        }

        public IServiceProvider? ServiceProvider { get; }
        IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider!; // TODO: Nullability of IHas?  ServiceProvider may be null.
    }

}
