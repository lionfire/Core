using LionFire.Structures;

namespace LionFire.Data.Id;

public class KeyedIdAdapterStrategy : IIdMappingStrategy
{
    public (bool, string) TryGetId(object obj)
    {
        if (obj is IKeyed k)
        {
            return (true, k.Key);
        }
        return (false, default);
    }
}
