using LionFire.Data;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public static class IReadWriteHandleExtensions
    {
        public static void SetValue(this IReadWriteHandle handle, object value/*, Type type = null*/)
        {
            //type ??= value?.GetType() ?? typeof(object);

            var setValueMethod = handle.GetType().GetProperty("Value").GetSetMethod();
            setValueMethod.Invoke(handle, new object[] { value });
        }

        public static async Task<object> TryGetOrCreate(this IReadWriteHandle handle, CancellationToken cancellationToken = default)
        {
            var result = (await handle.GetUnknownType(cancellationToken).ConfigureAwait(false));
            if (result.IsFound() == true) return result.Value;

            throw new NotImplementedException("TODO: Create");
        }

        // ENH: Return Put task separately and don't await it
        public static async Task<T> TryGetOrCreate<T>(this IReadWriteHandle<T> handle)  // REVIEW - return TransferResult<T>?  Generic doesn't exist yet.
        {
            if (handle is IGetter<T> lr)
            {
                var result = await lr.GetIfNeeded().ConfigureAwait(false);
                if (result.HasValue)
                {
                    return lr.Value;
                }
            }
            else
            {
                var result = (await handle.Get().ConfigureAwait(false));
                if (result.IsFound() == true) return result.Value;
            }

            handle.StagedValue = Activator.CreateInstance<T>();
            var putResult = await handle.Set().ConfigureAwait(false);
            if (putResult.IsSuccess != true)
            {
                throw new TransferException(putResult as ITransferResult, "Failed to create. Put result: " + putResult);
            }
            return handle.Value;
        }
    }
}
