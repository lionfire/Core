using LionFire.Dependencies;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using MorseCode.ITask;

namespace LionFire.Persisters.Expanders;

public interface IExpanderProvider : IResolverSync<IReference, IExpander>
{
    public IEnumerable<IExpander> Expanders { get; }
}
