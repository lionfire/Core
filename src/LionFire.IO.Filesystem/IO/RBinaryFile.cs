using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    public class RBinaryFile : RLocalFileBase<byte[]>
    {

        #region Construction

        public RBinaryFile() { }
        public RBinaryFile(string path) : base(path)
        {
        }

        #endregion

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
