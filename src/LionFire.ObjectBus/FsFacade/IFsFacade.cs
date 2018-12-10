using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.FsFacade
{
    // TODO: Backport this to FsOBase?
    public interface IFsFacade
    {
        Task<bool> Exists(string path);

        Task<bool?> Delete(string path);

        Task<IEnumerable<string>> GetKeys(string directoryPath, string pattern = null);
        Task<byte[]> ReadAllBytes(string path);
        Task<string> ReadAllText(string path);

        Task WriteAllBytes(string path, byte[] data);
        Task WriteAllText(string path, string data);
    }
    
}
