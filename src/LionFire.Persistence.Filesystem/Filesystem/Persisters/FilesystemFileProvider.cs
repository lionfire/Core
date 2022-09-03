using LionFire.IO;
using LionFire.Persistence.Filesystemlike;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystem;

public class FilesystemFileProvider : IFileProvider // TODO: Merge into LionFire.IO.VirtualFilesystem.dll
{
    public async Task<bool> Exists(string path)
        => await Task.Run(() => File.Exists(path)).ConfigureAwait(false);

    public Task<Stream> Open(string path, FileAccess fileAccess)
    {
        throw new System.NotImplementedException();
    }
}

