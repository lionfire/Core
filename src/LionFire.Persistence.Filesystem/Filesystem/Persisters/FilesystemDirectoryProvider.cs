using LionFire.Persistence.Filesystemlike;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystem
{

    // Is there a better way to do async?
    // https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i

    public class FilesystemDirectoryProvider : IDirectoryProvider
    {
        public Task<bool> Exists(string path)
            => Task.Run(() => Directory.Exists(path));

        public Task<string[]> GetFiles(string path)
            => Task.Run(() => Directory.GetFiles(path));
        public Task<string[]> GetDirectories(string path)
            => Task.Run(() => Directory.GetDirectories(path));
    }
    
}
