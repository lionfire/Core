using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{

    public interface IReader<TReference>: IOProvider
    {
        Task<Stream> ReadStream(TReference reference);
        Task<string> ReadString(TReference reference);
        Task<byte[]> ReadBytes(TReference reference);
    }
}
