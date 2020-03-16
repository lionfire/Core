using LionFire.Resolves;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public static class IReadWriteHandleExtensions
    {
        public static async Task<object> TryGetOrCreate(this IReadWriteHandle handle)
        {
            var result = (await handle.Resolve().ConfigureAwait(false)).ToRetrieveResult();
            if (result.IsFound() == true) return result.Value;

            throw new NotImplementedException("TODO: Create");
        }

        public static async Task<T> TryGetOrCreate<T>(this IReadWriteHandle<T> handle)  // REVIEW - return PersistenceResult<T>?  Generic doesn't exist yet.
        {
            var result = (await handle.Resolve().ConfigureAwait(false)).ToRetrieveResult();
            if (result.IsFound() == true) return result.Value;
            
            throw new NotImplementedException("TODO: Create");
        }
    }
}
