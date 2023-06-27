using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Async.Sets;

public interface ISetsRx<T> 
    : IObservableSets<T>
    , IStagesSet<T>

    , IHasNonNullSettable<AsyncValueOptions>

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
