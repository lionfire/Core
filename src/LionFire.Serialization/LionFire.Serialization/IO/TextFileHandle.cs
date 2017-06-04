using LionFire.Structures;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public class TextFileHandle : ReadHandleBase<string>, IHandle<string>
    {
        [SetOnce]
        public string Path { get => Key; set => Key = value; }


        public TextFileHandle(string path)
        {
            this.Path = path;
        }

        string IWriteHandle<string>.Object
        {
            set => File.WriteAllText(Path, value);
        }

        public Task Save(object persistenceContext = null)
        {
            File.WriteAllText(Path, Object);
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
