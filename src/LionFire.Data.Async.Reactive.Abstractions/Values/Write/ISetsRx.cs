using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Sets;

public interface ISetsRx<T> 
    : IObservableSets<T>
    , IStagesSet<T>

    , IHasNonNullSettable<AsyncSetOptions>

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
