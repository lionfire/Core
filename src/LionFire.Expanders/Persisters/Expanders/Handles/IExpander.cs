using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;

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
    Task<IReadHandle>? TryGetSourceReadHandle(IReference sourceReference);

    Task<IRetrieveResult<T>> RetrieveTarget<T>(IReadHandle sourceReadHandle, string targetPath);

}
