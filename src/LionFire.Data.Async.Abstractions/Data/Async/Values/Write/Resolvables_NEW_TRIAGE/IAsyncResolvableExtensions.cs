
namespace LionFire.Data.Async.Gets;

public static class IAsyncResolvableExtensions
{
    public static bool IsResolutionExpensive(this IGetter ar)
    {
        if (ar is IGetterEx are)
        {
            return are.IsResolutionExpensive;
        }
        return true;
    }
}
