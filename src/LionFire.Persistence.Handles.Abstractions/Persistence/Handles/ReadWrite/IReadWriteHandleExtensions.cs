using System;

namespace LionFire.Persistence
{
    public static class IReadWriteHandleExtensions
    {
        public static T TryGetOrCreate<T>(this IReadWriteHandle<T> handle)
        {
            throw new NotImplementedException();
        }
    }
}
