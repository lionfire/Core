
namespace LionFire.Data.Collections;

public interface IEnumerableGetter<TItem>
    : IGetter<IEnumerable<TItem>>
    , IEnumerable<TItem>
{
    
}
