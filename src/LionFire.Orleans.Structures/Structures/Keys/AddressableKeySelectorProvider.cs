namespace LionFire.Orleans_;

public static class AddressableKeySelectorProvider
{
    public static Func<TItem,TKey>? TryGetKeySelector<TItem,TKey>()
        where TKey : notnull
    {
        if(typeof(TKey) == typeof(GrainId) && typeof(TItem).IsAssignableTo(typeof(IAddressable)))
        {
            return item =>
            {
                ArgumentNullException.ThrowIfNull(item);
                return (TKey)(object)(((IAddressable)item).GetGrainId()); // HARDCAST
            };
        }
        return null;
    }
}
