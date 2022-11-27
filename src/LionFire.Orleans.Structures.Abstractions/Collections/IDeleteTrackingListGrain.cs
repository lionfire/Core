namespace LionFire.Orleans_.Collections;

public interface IDeleteTrackingListGrain<TKey>
{
    Task<IEnumerable<KeyValuePair<DateTime, TKey>>> DeletedKeys();
}
