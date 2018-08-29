using LionFire.Structures;

namespace LionFire.Referencing
{
    public interface H<T> : R<T>, IWrapper<T>
    {
        new T Object { get; set; }
    }

}
