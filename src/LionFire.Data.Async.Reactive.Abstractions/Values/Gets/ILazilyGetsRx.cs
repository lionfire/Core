using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Async.Gets;

public interface ILazilyGetsRx<T> 
    : ILazilyGets<T>
    , IHasNonNullSettable<AsyncGetOptions>

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
