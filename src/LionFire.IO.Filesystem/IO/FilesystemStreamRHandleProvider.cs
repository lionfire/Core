using System;
using System.IO;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.IO.Filesystem;

public class FilesystemStreamReadHandleProvider : FilesystemHandleProviderBase, IReadHandleProvider
{
    public override string Scheme => "file-stream-ro";

    public IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default)
    {
        if (typeof(T) != typeof(Stream))
        {
            throw new ArgumentException("Object type must be Stream");
        }

        ValidateReference(reference);

        return (IReadHandle<T>)new RFileStream(reference.Path, (Stream)(object)preresolvedValue); // HARDCAST
    }

    public IReadHandle<T> GetReadHandle<T>(IReference reference) => GetReadHandle<T>(reference, default);
}
