#nullable enable


using LionFire.Data.Gets;
using LionFire.Data.Reactive;
using LionFire.Data.Sets;

namespace LionFire.Data.Mvvm;

public class ValueVM<T> 
    : GetterVM<T> // TEMP
                
                //, IStagesSet<T>
                //, IValueVM<T>
, IViewModel<IGetter<T>>
{

    public Value<T> Value { get; }
}
