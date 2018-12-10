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


        protected override Task WriteObject(object persistenceContext = null)
        {
            File.WriteAllBytes(Path, Object);
            return Task.CompletedTask;
        }

        protected override async Task<bool?> DeleteObject(object persistenceContext = null)
        {
            return await Task.Run(() =>
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    return true;
                }
                return false;
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
