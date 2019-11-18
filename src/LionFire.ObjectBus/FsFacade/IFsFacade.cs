using LionFire.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.FsFacade
{
    // TODO: Backport this to FSOBase?
    public interface IFsFacade
    {
        Task<bool> Exists(string path);

        Task<IPersistenceResult> Delete(string path);

        Task<IEnumerable<string>> List(string directoryPath, string pattern = null);
        Task<byte[]> ReadAllBytes(string path);
        Task<string> ReadAllText(string path);

        Task WriteAllBytes(string path, byte[] data);
        Task WriteAllText(string path, string data);
    }
    
}
