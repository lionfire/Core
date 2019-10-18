namespace LionFire.Resolvables
{
    public static class IAsyncResolvableExtensions
    {
        public static bool IsResolutionExpensive(this IAsyncResolvable ar)
        {
            if (ar is IAsyncResolvableEx are)
            {
                return are.IsResolutionExpensive;
            }
            return true;
        }
    }
}
