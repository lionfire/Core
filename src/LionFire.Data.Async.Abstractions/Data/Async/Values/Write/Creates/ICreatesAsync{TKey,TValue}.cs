namespace LionFire.Data.Async.Sets;

public interface ICreatesAsync<TKey, TValue>
{
    public Task<TValue> CreateForKey(TKey key, Type type, params object[] constructorParameters);

    public Task<TValue> GetOrCreateForKey(TKey key, Type type, params object[] constructorParameters);
}
