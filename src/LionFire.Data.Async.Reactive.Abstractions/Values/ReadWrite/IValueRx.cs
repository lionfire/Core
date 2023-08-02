using LionFire.Data.Gets;
using LionFire.Data.Sets;
using LionFire.Ontology;

namespace LionFire.Data.Reactive;

public interface IValueRx<T> 
    : IGetsRx<T>
    , ISetsRx<T>
    , IValue<T>
//, IHasNonNullSettable<AsyncValueOptions>
{
    AsyncValueOptions Options { get; set; }
}
