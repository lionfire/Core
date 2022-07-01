#nullable enable
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystemlike
{
    /// <summary>
    /// Analogous interface to System.IO.File.*
    /// </summary>
    public interface IFileProvider
    {
        Task<bool> Exists(string path);
    }

}
