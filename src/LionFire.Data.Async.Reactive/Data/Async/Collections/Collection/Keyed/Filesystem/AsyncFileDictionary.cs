
namespace LionFire.Data.Collections.Filesystem;

public class AsyncFileDictionary : AsyncDictionary<string, byte[]>
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
    }

    #endregion

    #region Read

    protected override ITask<IGetResult<IEnumerable<KeyValuePair<string, byte[]>>>> GetImpl(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Write

    public override async ValueTask<bool> Remove(string key)
        => await Task.Run(() =>
        {
            var exists = File.Exists(key);
            File.Delete(key);
            return exists;
        });

    public override async ValueTask Add(Label<string, byte[]> value)
    {
        await Task.Run(() =>
        {
            var path = PathForName(value.Key);
            if (File.Exists(path)) throw new AlreadyException();
            if (value.Value != null)
            {
                File.WriteAllBytes(path, value.Value);
            }
        });
    }

    public override async ValueTask Upsert(Label<string, byte[]> value)
    {
        await Task.Run(() =>
        {
            var path = PathForName(value.Key);
            if (value.Value == null)
            {
                File.Delete(value.Key);
            }
            else
            {
                File.WriteAllBytes(path, value.Value);
            }
        });
    }

    public override ValueTask<bool> TryAdd(string key, byte[] item)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool?> Upsert(string key, byte[] item)
    {
        throw new NotImplementedException();
    }

    #endregion

    
}



