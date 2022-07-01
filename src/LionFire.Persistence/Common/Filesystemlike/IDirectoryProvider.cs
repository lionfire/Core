#nullable enable
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystemlike
{
    //public interface IFilesystemProvider
    //{
    //    IDirectoryProvider DirectoryProvider { get; }
    //    IFileProvider FileProvider { get; }
    //}

    /// <summary>
    /// Analogous interface to System.IO.Directory.*
    /// </summary>
    public interface IDirectoryProvider
    {
        Task<bool> Exists(string path);
        Task<string[]> GetFiles(string path);
    Task<string[]> GetDirectories(string path);
    }


}
