#nullable enable

namespace LionFire.Structures;

public interface IDependsOn<T>
{
    T Dependency { set; }
}
