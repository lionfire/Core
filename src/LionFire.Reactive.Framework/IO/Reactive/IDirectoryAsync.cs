// ENH maybe: for now just support the one approach of nameless documents.  Uncommenting this would allow for the document file to carry the matching name of the parent directory, which may make for more ease of use in some situations (such as having multiple files open in an editor.)
//#define DocumentWithMatchingKeyName
using LionFire.Structures;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace LionFire.IO.Reactive;

public interface IDirectoryAsync
{
    Task<bool> ExistsAsync(string path);
    Task CreateDirectoryAsync(string path);

    Task<string[]> GetFilesAsync(string path);

    Task<string[]> GetFilesAsync(string path, string searchPattern);

    // REVIEW - eliminate this overload with System.IO.SearchOption?
    Task<string[]> GetFilesAsync(string path, string searchPattern, System.IO.SearchOption searchOption);
    Task<string[]> GetFilesAsync(string path, string searchPattern, System.IO.EnumerationOptions enumerationOptions);
}

public class FsDirectory : IDirectoryAsync
{
    public static FsDirectory Instance => Singleton<FsDirectory>.Instance;

    public Task<bool> ExistsAsync(string path) => Task.Run(() => Directory.Exists(path));
    public Task CreateDirectoryAsync(string path) => Task.Run(() => Directory.CreateDirectory(path));

    public Task<string[]> GetFilesAsync(string path) => Task.Run(() => Directory.GetFiles(path));
    public Task<string[]> GetFilesAsync(string path, string searchPattern) => Task.Run(() => Directory.GetFiles(path, searchPattern));

    public Task<string[]> GetFilesAsync(string path, string searchPattern, System.IO.SearchOption searchOption)
        => Task.Run(() => Directory.GetFiles(path, searchPattern, searchOption));
    public Task<string[]> GetFilesAsync(string path, string searchPattern, System.IO.EnumerationOptions enumerationOptions)
        => Task.Run(() => Directory.GetFiles(path, searchPattern, enumerationOptions));

}
