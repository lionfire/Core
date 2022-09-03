
namespace LionFire.IO;

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

#if false  // OLD - do this in Vos instead

public interface IArchivePlugin
{
    IEnumerable<string> Extensions { get; }
    Task<bool> Exists(string pathToArchive, string pathInArchive);
    Task<string> ReadAllText(string pathToArchive, string pathInArchive);
    Task Write(string pathToArchive, string pathInArchive, string textContents);
}

public class ZipArchiveProvider : IArchivePlugin
{
    public IEnumerable<string> Extensions { get { yield return "zip"; } }

    public Task<bool> Exists(string pathToArchive, string pathInArchive)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadAllText(string pathToArchive, string pathInArchive)
    {
        throw new NotImplementedException();
    }
    public Task Write(string pathToArchive, string pathInArchive, string textContents)
    {
        throw new NotImplementedException();
    }
}

public class ArchiveFilesystem : IFilesystemPlugin
{
    public IEnumerable<IArchivePlugin> ArchivePlugins { get; }

    public ArchiveFilesystem(IEnumerable<IArchivePlugin> archivePlugins)
    {
        ArchivePlugins = archivePlugins;
    }

#region IFilesystem

    /// <summary>
    /// Discovering virtual directories that exist because of archives
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public IEnumerable<string> GetDirectories(string path)
    {
        throw new NotImplementedException();
    }

#endregion

#region 


#endregion

#region Archives populating a particular directory

    public IEnumerable<string> PossibleArchiveLocationsForDirectory(string directory)
    {

    }

    public IEnumerable<string> ArchivesForDirectory(string directory)
    {

    }

#endregion

    public bool IsArchive(string path)
    {

    }
}

public interface IFilesystemPlugin
{

}

#endif
