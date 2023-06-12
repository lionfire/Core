using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Async.Sets;

public interface ISetsRx<T> 
    : ISets<T>
    , IStagesSet<T>
    , IObservableSets<T>

    , IHasNonNullSettable<AsyncValueOptions>

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
