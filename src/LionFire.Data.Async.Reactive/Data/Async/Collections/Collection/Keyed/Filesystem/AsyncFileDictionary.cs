#if UNUSED
using DynamicData;
using LionFire.Data.Async;

namespace LionFire.Data.Collections.Filesystem;

public class AsyncFileDictionary<TValue> : AsyncReadOnlyDictionary<string, TValue>
    where TValue : notnull
{
    #region Parameters

    public string Dir { get; }

    #region Derived

    public string PathForName(string name) => System.IO.Path.Combine(Dir, name);

    #endregion

    #endregion

    #region Lifecycle

    public AsyncFileDictionary(string dir)
    {
        Dir = dir;
    }1

    #endregion

    #region Read

    //protected virtual ITask<IGetResult<IEnumerable<KeyValuePair<string, byte[]>>>> GetImpl(CancellationToken cancellationToken = default)
    //{
    //    throw new NotImplementedException();
    //}

    #endregion

    #region Write

    public virtual async ValueTask<bool> Remove(string key)
        => await Task.Run(() =>
        {
            var exists = File.Exists(key);
            File.Delete(key);
            return exists;
        });

    public virtual async ValueTask<bool> TryAdd(string key, byte[] value)
    {
        var path = PathForName(key);
        if (File.Exists(path)) return false;
        ArgumentNullException.ThrowIfNull(value);
        await File.WriteAllBytesAsync(path, value);
        return true;
    }

    public virtual async ValueTask Upsert(string key, byte[] value)
    {
        var path = PathForName(key);
        if (value == null)
        {
            File.Delete(key);
        }
        else
        {
            await File.WriteAllBytesAsync(path, value);
        }
    }

 
    #endregion


}




#endif