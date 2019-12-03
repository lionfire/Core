using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    public interface IWriter<TReference>: IOProvider
    {
        Task<Stream> WriteStream(TReference reference, ReplaceMode replaceMode = ReplaceMode.Upsert);
        Task WriteString(TReference reference, string str, ReplaceMode replaceMode = ReplaceMode.Upsert);
        Task WriteBytes(TReference reference, byte[] bytes, ReplaceMode replaceMode = ReplaceMode.Upsert);
    }
}
