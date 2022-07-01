using LionFire.Persistence.Filesystemlike;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystem
{
    public class FilesystemFileProvider : IFileProvider
    {
        public async Task<bool> Exists(string path)
            => await Task.Run(() => File.Exists(path)).ConfigureAwait(false);
    }

}
