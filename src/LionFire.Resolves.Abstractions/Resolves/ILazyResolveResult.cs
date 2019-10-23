using LionFire.Results;

namespace LionFire.Resolves
{
    public interface ILazyResolveResult<out TValue> : IResolveResult<TValue>, INoopResult
    {
    }
}
