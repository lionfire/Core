using LionFire.Data.Gets;
using LionFire.Data.Sets;
using LionFire.Ontology;

namespace LionFire.Data.Reactive;

public interface IAsyncValueRx<T> 
    : ILazilyGetsRx<T>
    , ISetsRx<T>
    , IHasNonNullSettable<AsyncValueOptions>
{
}
