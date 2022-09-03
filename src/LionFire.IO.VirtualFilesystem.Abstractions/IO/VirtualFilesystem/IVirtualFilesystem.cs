namespace LionFire.IO;

public interface IVirtualFilesystemDirectories
{
    Task<IEnumerable<string>> GetDirectories(string path);
    Task<bool> DirectoryExists(string path);


}


public interface IVirtualFilesystem : IVirtualFilesystemDirectories
{
    

    Task<IEnumerable<string>> GetFiles(string path);
    Task<bool> FileExists(string path);

    Task<string> ReadAllText(string path, CancellationToken cancellationToken = default);
    Task WriteText(string path, string? contents, CancellationToken cancellationToken = default);
    Task DeleteFile(string path);
}


/// <summary>
/// Analogous interface to System.IO.File.*
/// </summary>
[Obsolete]
public interface IFileProvider // REFACTOR:  merge with IVirtualFilesystem
{
    Task<bool> Exists(string path);

    Task<Stream> Open(string path, FileAccess fileAccess);
}


/// <summary>
/// Analogous interface to System.IO.Directory.*
/// </summary>
[Obsolete]
public interface IDirectoryProvider   // REFACTOR Merge/replace with LionFire.IO.IVirtualFilesystem
{
    Task<bool> Exists(string path);
    Task<string[]> GetFiles(string path);
    Task<string[]> GetDirectories(string path);
}
