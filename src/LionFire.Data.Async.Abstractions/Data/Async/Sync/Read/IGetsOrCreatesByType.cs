
namespace LionFire.Data.Async.Gets;

public interface IGetsOrCreatesByType
{
    T GetOrCreate<T>() where T : class;
}
