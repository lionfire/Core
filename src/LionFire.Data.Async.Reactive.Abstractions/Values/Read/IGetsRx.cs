using LionFire.Ontology;
using LionFire.Reactive;
using System.Reactive;

namespace LionFire.Data.Gets;

public interface IGetsRx<out TValue> 
    : IGets<TValue> // ReadCacheValue
    , IHasNonNullSettable<AsyncGetOptions> // TODO: Remove this, just have interface member?
    , IReactiveObjectEx
    , IObservableGetResults<TValue>
{    
}
