using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Referencing
{
    public interface H<T> : RH<T>, IWrapper<T>, ICommitable, IDeletable, WH<T>
    {
        new T Object { get; set; }
    }

}
