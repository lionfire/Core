using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IValueRx<T> 
    : IGetterRxO<T>
    , ISetterRxO<T>
    , IValue<T>
//, IHasNonNullSettable<AsyncValueOptions>
{
    ValueOptions Options { get; set; }
}
