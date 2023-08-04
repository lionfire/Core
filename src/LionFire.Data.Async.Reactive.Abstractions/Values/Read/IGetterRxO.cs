using LionFire.Ontology;
using LionFire.Reactive;

namespace LionFire.Data.Async.Gets;

public interface IGetterRxO<out TValue> 
    : IReactiveObjectEx
    , IGetter<TValue> // ReadCacheValue
    //, IHasNonNullSettable<GetterOptions> // TODO: Remove this, just have interface member?
    , IObservableGetResults<TValue>
    , IObservableGetOperations<TValue>
{
}
