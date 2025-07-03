using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.DynamicData_;

public static class SourceCacheX
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Semaphore:
    ///   private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    /// </remarks>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="semaphore"></param>
    /// <returns></returns>
    public static async Task<TObject> GetOrAddAsync<TObject, TKey>(this SourceCache<TObject, TKey> cache, TKey key, Func<TKey, Task<TObject>> factory, SemaphoreSlim semaphore)
        where TObject : class
        where TKey : notnull
    {
        var optional = cache.Lookup(key);
        if (optional.HasValue)
            return optional.Value;

        await semaphore.WaitAsync();
        try
        {
            optional = cache.Lookup(key);
            if (optional.HasValue)
                return optional.Value;

            var item = await factory(key);
            cache.AddOrUpdate(item);
            return item;
        }
        finally
        {
            semaphore.Release();
        }
    }

}
