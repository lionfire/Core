using LionFire.Structures;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public class BinaryFileHandle : ReadHandleBase<byte[]>, IHandle<byte[]>, IWriteHandle<byte[]>
    {
        [SetOnce]
        public string Path { get => Key; set => Key = value;}

        public BinaryFileHandle(string path)
        {
            this.Path = path;
        }

        byte[] IWriteHandle<byte[]>.Object
        {
            set => File.WriteAllBytes(Path, value);
        }

        public Task Save(object persistenceContext = null)
        {
            File.WriteAllBytes(Path, Object);
            return Task.CompletedTask;
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            if (!File.Exists(Path)) return Task.FromResult(false);
            this._object = File.ReadAllBytes(Path);
            return Task.FromResult(true);
        }
    }
}
