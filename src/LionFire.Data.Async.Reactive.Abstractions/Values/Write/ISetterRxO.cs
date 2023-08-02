using LionFire.Data.Gets;
using LionFire.Ontology;
using LionFire.Reactive;
using ReactiveUI;

namespace LionFire.Data.Sets;

/// <summary>
/// ReactiveObject (ReactiveUI) version of ISetter
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISetterRxO<T> 
    : IReactiveObjectEx
    , ISets<T> // contravariant
    , IObservableSetResults // non-generic
    , IObservableSetOperations<T> // covariant
    , IStagesSet<T> // covariant and contravariant

    , IHasNonNullSettable<AsyncSetOptions> // TODO? Or just have an interface member?
{
}
