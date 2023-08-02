#nullable enable


using LionFire.Data.Gets;
using LionFire.Data.Reactive;
using LionFire.Data.Sets;

namespace LionFire.Data.Mvvm;

public class ValueVM<T> 
    : GetsVM<T> // TEMP
                
                //, IStagesSet<T>
                //, IValueVM<T>
, IViewModel<IGets<T>>
{

    public Value<T> Value { get; }
}
