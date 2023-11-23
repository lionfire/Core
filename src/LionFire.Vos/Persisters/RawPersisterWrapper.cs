using LionFire.IO;
using LionFire.Referencing;
using System.IO;

namespace LionFire.Persistence.Persisters.Vos;

public class RawPersisterWrapper<TReference> : IReader<TReference>, IWriter<TReference>
    where TReference : IReference
{

    public IOCapabilities Capabilities => throw new NotImplementedException();

    public Task<byte[]> ReadBytes(TReference reference)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> ReadStream(TReference reference)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadString(TReference reference)
    {
        throw new NotImplementedException();
    }

    public Task WriteBytes(TReference reference, byte[] bytes, ReplaceMode replaceMode = ReplaceMode.Upsert)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> WriteStream(TReference reference, ReplaceMode replaceMode = ReplaceMode.Upsert)
    {
        throw new NotImplementedException();
    }

    public Task WriteString(TReference reference, string str, ReplaceMode replaceMode = ReplaceMode.Upsert)
    {
        throw new NotImplementedException();
    }
}
