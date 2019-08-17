using LionFire.ObjectBus.FsFacade;
using LionFire.Persistence;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    // TODO: Make async
    public class FsFsFacade : IFsFacade
    {
        public async Task<IPersistenceResult> Delete(string path)
        {
            var exists = await Exists(path);
            if (exists)
            {
                File.Delete(path);
                return PersistenceResult.Success;
            }
            else
            {
                return PersistenceResult.NotFound;
            }
        }
        public Task<bool> Exists(string path) => Task.FromResult(File.Exists(path));
        public Task<IEnumerable<string>> GetFiles(string path, string pattern = null) => Task.FromResult<IEnumerable<string>>(Directory.GetFiles(path, pattern));
        public Task<IEnumerable<string>> List(string directoryPath, string pattern = null) => throw new System.NotImplementedException();
        public Task<byte[]> ReadAllBytes(string path) => Task.FromResult(File.ReadAllBytes(path));
        public Task<string> ReadAllText(string path) => Task.FromResult(File.ReadAllText(path));
        public Task WriteAllBytes(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
            return Task.CompletedTask;
        }
        public Task WriteAllText(string path, string data)
        {
            File.WriteAllText(path, data);
            return Task.CompletedTask;
        }
    }
}
