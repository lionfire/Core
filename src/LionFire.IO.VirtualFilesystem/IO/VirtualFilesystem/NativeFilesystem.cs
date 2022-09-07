
namespace LionFire.IO;

// Is there a better way to do async?
// https://stackoverflow.com/questions/719020/is-there-an-async-version-of-directoryinfo-getfiles-directory-getdirectories-i


public class NativeFilesystem : IVirtualFilesystem
{
    #region Directories

    public Task<IEnumerable<string>> GetDirectories(string path) => Task.Run(() => (IEnumerable<string>)Directory.GetDirectories(path));
    public Task<IEnumerable<string>> GetDirectories(string path, string searchPattern) => Task.Run(() => (IEnumerable<string>)Directory.GetDirectories(path, searchPattern));

    public Task<bool> DirectoryExists(string path) => Task.Run(() => Directory.Exists(path));

    public Task<IEnumerable<string>> GetFiles(string path) => Task.Run(() => (IEnumerable<string>)Directory.GetFiles(path));

    #endregion

    #region File

    public Task<bool> FileExists(string path) => Task.Run(() => File.Exists(path));

    public Task<string> ReadAllText(string path, CancellationToken cancellationToken = default) => File.ReadAllTextAsync(path, cancellationToken);
    public Task WriteText(string path, string? contents, CancellationToken cancellationToken = default) => File.WriteAllTextAsync(path, contents, cancellationToken);

    public Task DeleteFile(string path) => Task.Run(() => File.Delete(path));

    #endregion


}


//public class FilesystemFileProvider : IFileProvider // TODO: Merge into LionFire.IO.VirtualFilesystem.dll
//{
//    public async Task<bool> Exists(string path)
//        => await Task.Run(() => File.Exists(path)).ConfigureAwait(false);

//    public Task<Stream> Open(string path, FileAccess fileAccess)
//    {
//        throw new System.NotImplementedException();
//    }
//}

