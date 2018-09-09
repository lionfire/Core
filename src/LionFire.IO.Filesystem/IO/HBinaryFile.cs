using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{

    public class HBinaryFile : HLocalFileBase<byte[]>
    {

        #region Construction

        public HBinaryFile() { }
        public HBinaryFile(string path) : base(path)
        {
        }

        #endregion


        public override Task WriteObject(object persistenceContext = null)
        {
            File.WriteAllBytes(Path, Object);
            return Task.CompletedTask;
        }

        public override async Task DeleteObject(object persistenceContext = null)
        {
            await Task.Run(() =>
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }).ConfigureAwait(false);
        }

        public override async Task<bool> TryRetrieveObject()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return false;
                }

                OnRetrievedObject(File.ReadAllBytes(Path));
                return true;
            }).ConfigureAwait(false);
        }
    }
}
