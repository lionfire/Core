namespace LionFire.Orleans_.Collections;


public interface IListGrain<TValue, TMetadata>
    where TValue : IGrain
{
    Task<bool> Remove(string id);
    Task<(string id, TValue newValue)> Create(Type type);

    Task<IEnumerable<Type>> CreateableTypes();

    Task<IEnumerable<GrainListItem<TValue, TMetadata>>> Items();

}

public interface IEnumerableGrain<TValue>
    where TValue : class
{
    Task<IEnumerable<TValue>> Items();
}

public interface IListGrain<TValue> : IEnumerableGrain<TValue>
    where TValue : class
{
    Task<bool> Remove(string id);
    Task<TValue> Create(Type type);

    Task<IEnumerable<Type>> CreateableTypes();

}
