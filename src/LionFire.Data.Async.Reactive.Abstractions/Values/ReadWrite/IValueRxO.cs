using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using LionFire.Ontology;

namespace LionFire.Data.Async;

public interface IValueRxO<T> 
    : IGetterRxO<T>
    , ISetterRxO<T>
    , IValue<T>
    //, IHasNonNullSettable<ValueOptions>
{
    ValueOptions Options { get; set; }
}
