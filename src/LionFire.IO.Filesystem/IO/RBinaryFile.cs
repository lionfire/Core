using LionFire.Persistence;
using LionFire.Data.Gets;
using MorseCode.ITask;
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

        protected override async ITask<IGetResult<byte[]>> GetImpl(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return RetrieveResult<byte[]>.NotFound;
                }

                //return RetrieveResult<byte[]>.Success(OnRetrievedObject(File.ReadAllBytes(Path)));
                return RetrieveResult<byte[]>.Success(File.ReadAllBytes(Path));
            }).ConfigureAwait(false);
        }
    }
}
