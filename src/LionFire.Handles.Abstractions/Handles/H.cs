using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Referencing
{
    public interface H<T> : R<T>, IWrapper<T>, ISaveable
    {
        new T Object { get; set; }
    }

}
