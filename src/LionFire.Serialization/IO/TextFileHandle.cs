using LionFire.Structures;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public class TextFileHandle : WritableHandleBase<string>
    {
        [SetOnce]
        public string Path { get => Key; set => Key = value; }


        public TextFileHandle(string path)
        {
            this.Path = path;
        }

        public override Task WriteObject(object persistenceContext = null)
        {
            File.WriteAllText(Path, Object);
            return Task.CompletedTask;
        }

        public override Task DeleteObject(object persistenceContext = null)
        {
            if (File.Exists(Path)) File.Delete(Path);
            return Task.CompletedTask;
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            if (!File.Exists(Path)) return Task.FromResult(false);
            this._object = File.ReadAllText(Path);
            return Task.FromResult(true);
        }
    }
}
