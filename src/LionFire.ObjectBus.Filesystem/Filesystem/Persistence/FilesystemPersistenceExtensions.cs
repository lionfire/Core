//#define TRACE_SAVE
#define TRACE_LOAD

using System.IO;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Persistence.Filesystem
{
    public static class FilesystemPersistenceExtensions
    {
        public static async Task<Stream> PathToReadStream(this string path) => await Task.Run(() => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)).ConfigureAwait(false);
        public static async Task<byte[]> PathToBytes(this string path) => await Task.Run(() => File.ReadAllBytes(path)).ConfigureAwait(false);
        public static async Task<string> PathToString(this string path) => await Task.Run(() => File.ReadAllText(path)).ConfigureAwait(false);
    }
}
