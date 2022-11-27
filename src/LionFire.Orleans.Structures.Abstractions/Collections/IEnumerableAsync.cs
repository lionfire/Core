namespace LionFire.Structures;

public interface IEnumerableAsync<TValue>  // MOVE to LionFire.Structures
{
    Task<IEnumerable<TValue>> GetEnumerableAsync();
}
