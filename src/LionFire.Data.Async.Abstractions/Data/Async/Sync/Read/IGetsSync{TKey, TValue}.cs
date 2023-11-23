
namespace LionFire.Data.Async.Gets;

public interface IGetsSync<TKey, TValue>
{
    IGetResult<TValue> Resolve(TKey reference);
}
