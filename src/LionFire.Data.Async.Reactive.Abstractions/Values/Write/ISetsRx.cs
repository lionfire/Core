using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Sets;

public interface ISetsRx<T> 
    : ISets<T> // contravariant
    , IObservableSets<T> // covariant
    , IStagesSet<T>

    //, IHasNonNullSettable<AsyncSetOptions>

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
