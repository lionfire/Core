using LionFire.Persistence;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

    public static class IReadHandleExtensions
    {
        public static async Task<bool> TryLoad(this IReadHandle<object> rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return true;
            }).ConfigureAwait(false);
        }
        public static async Task<bool> TryLoadNonNull(this IReadHandle<object> rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return _ != null;
            }).ConfigureAwait(false);
        }
        public static bool IsWritable<T>(this IReadHandle<T> readHandle)
        {
            return readHandle as ISaveable != null;
        }
    }

}
