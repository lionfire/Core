using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;


namespace LionFire.Data.Async;

public interface IStatelessGetterAndSetter<TValue>
{
    IStatelessGetter<TValue> Getter { get; }
    ISetter<TValue> Setter { get; }
}
