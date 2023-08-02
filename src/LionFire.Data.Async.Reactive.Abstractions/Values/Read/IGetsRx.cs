using LionFire.Ontology;
using ReactiveUI;

namespace LionFire.Data.Gets;

public interface IGetsRx<out T> 
    : ILazilyGets<T>
    , IHasNonNullSettable<AsyncGetOptions> // TODO: Remove this, just have interface member?

    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}
