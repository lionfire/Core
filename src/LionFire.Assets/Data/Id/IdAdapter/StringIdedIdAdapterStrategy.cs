using LionFire.Structures;

namespace LionFire.Data.Id
{

    public class StringIdedIdAdapterStrategy : IIdMappingStrategy
    {
        public (bool, string) TryGetId(object obj)
        {
            if (obj is IIded<string> k)
            {
                return (true, k.Id);
            }
            return (false, default);
        }
    }
}
