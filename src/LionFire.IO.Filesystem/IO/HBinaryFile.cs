using LionFire.Persistence;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    public class HBinaryFile : HLocalFileBase<byte[]>
    {

        #region Construction

        public HBinaryFile() { }
        public HBinaryFile(string path, byte[] initialData = default) : base(path, initialData)
        {
        }

        #endregion
                
        protected override async Task<IPersistenceResult> WriteObject()
        {
            await Task.Run(() =>
            {
                File.WriteAllBytes(Path, Object);
            });
            return PersistenceResult.Success;
        }

        protected override async Task<IPersistenceResult> DeleteObject()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) return PersistenceResult.NotFound;

                File.Delete(Path);
                return PersistenceResult.Found;
            }).ConfigureAwait(false);
        }

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
