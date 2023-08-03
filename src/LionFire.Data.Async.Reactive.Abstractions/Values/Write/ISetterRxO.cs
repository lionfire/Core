using LionFire.Ontology;
using LionFire.Reactive;

namespace LionFire.Data.Async.Sets;

/// <summary>
/// ReactiveObject (ReactiveUI) version of ISetter, implementing IStagesSet
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface ISetterRxO<TValue> 
    : IReactiveObjectEx
    , ISetter<TValue> // contravariant
    , IObservableSetResults<TValue> // covariant
    , IObservableSetOperations<TValue> // covariant
    , IStagesSet<TValue> // covariant and contravariant
    , IHasNonNullSettable<SetterOptions> // TODO? Or just have an interface member?
{
}
