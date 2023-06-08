using LionFire.Results;

namespace LionFire.Data.Async.Gets
{
    public interface ILazyResolveResult<out TValue> : IGetResult<TValue>, INoopResult
    {
    }
}
