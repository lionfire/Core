using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Sets;

public interface ISetsRx<T> 
    : ISets<T> // contravariant
    , IObservableSets<T> // covariant
    , IStagesSet<T>

    //, IHasNonNullSettable<AsyncSetOptions> // TODO? Or just have an interface member?

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
