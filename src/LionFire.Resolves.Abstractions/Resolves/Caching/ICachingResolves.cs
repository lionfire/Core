using LionFire.Structures;

namespace LionFire.Resolves
{
    /// <summary>
    /// Like IReadHandle but without the Reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachingResolves<out T> : IReadWrapper<T>, IResolves<T>, IDefaultable
    {
    }
}
