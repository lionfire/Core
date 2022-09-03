namespace LionFire.Orleans_.Collections;

public interface IListGrain<TValue, TMetadata>
    where TValue : IGrain
{
    Task<bool> Remove(string id);
    Task<(string id, TValue newValue)> Create(Type type);

    Task<IEnumerable<Type>> CreateableTypes();

    Task<IEnumerable<GrainListItem<TValue, TMetadata>>> Items();

}

public interface IListGrain<TValue>
    where TValue : IGrain
{
    Task<bool> Remove(string id);
    Task<(string id, TValue newValue)> Create(Type type);

    Task<IEnumerable<Type>> CreateableTypes();

    Task<IEnumerable<GrainListItem<TValue>>> Items();

}
