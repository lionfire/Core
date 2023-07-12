
namespace LionFire.Data.Gets;

public interface IGetsSync<TKey, TValue>
{
    IGetResult<TValue> Resolve(TKey reference);
}
