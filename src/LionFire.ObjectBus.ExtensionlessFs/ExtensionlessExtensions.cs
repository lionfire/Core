using System.IO;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Persistence.ExtensionlessFilesystem
{
    public static class ExtensionlessExtensions
    {
        public static Task<Stream> PathToReadStream(string path) => Task.Run(() => (Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        public static Task<byte[]> PathToBytes(string path) => Task.Run(() => File.ReadAllBytes(path));
        public static Task<string> PathToString(string path) => Task.Run(() => File.ReadAllText(path));
    }
}
