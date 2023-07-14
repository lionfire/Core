
namespace LionFire.Data.Gets;

public interface IGetsOrCreatesByType
{
    T GetOrCreate<T>() where T : class;
}
