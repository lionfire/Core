using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

public interface IExpander
{
    /// <summary>
    /// If null, defer to SourceReadTypeForReference
    /// </summary>
    /// <returns></returns>
    Type? SourceReadType();

    /// <summary>
    /// If null, reference is not supported
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    Type? SourceReadTypeForReference(IReference reference);
    IReadHandle? TryGetReadHandle(IReference sourceReference);
}
