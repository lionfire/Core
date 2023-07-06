#if UNUSED
using LionFire.Structures;

namespace LionFire.Data.Gets;

/// <summary>
/// Like IReadHandle but without the Reference.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICachingGets<out T> : IReadWrapper<T>, IGets<T>, IDefaultable
{
}
#endif