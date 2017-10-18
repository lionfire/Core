using LionFire.Structures;
using System.IO;
using System.Threading.Tasks;
using LionFire.Handles;

namespace LionFire.IO
{
    public class BinaryFileHandle : WritableHandleBase<byte[]>
    {
        [SetOnce]
        public string Path { get => Key; set => Key = value;}

        public BinaryFileHandle(string path)
        {
            this.Path = path;
        }

        public override Task WriteObject(object persistenceContext = null)
        {
            File.WriteAllBytes(Path, Object);
            return Task.CompletedTask;
        }

        public override Task DeleteObject(object persistenceContext = null)
        {
            if(File.Exists(Path)) File.Delete(Path);
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
