using LionFire.Data.Async;
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Mvvm;

public class ValueVM<T> 
    : GetterVM<T> // TEMP
                
                //, IStagesSet<T>
                //, IValueVM<T>
    , IViewModel<IGetter<T>>
{

    public Value<T> Value { get; }
    IGetter<T> IReadWrapper<IGetter<T>>.Value => Value;

}
