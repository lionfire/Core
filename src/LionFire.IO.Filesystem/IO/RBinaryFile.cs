using LionFire.Persistence;
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

        public override async Task<IRetrieveResult<byte[]>> RetrieveImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return RetrieveResult<byte[]>.NotFound;
                }

                return RetrieveResult<byte[]>.Success(OnRetrievedObject(File.ReadAllBytes(Path)));
            }).ConfigureAwait(false);
        }
    }
}
