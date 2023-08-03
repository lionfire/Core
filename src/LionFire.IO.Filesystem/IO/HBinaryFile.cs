using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using System.IO;

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

        protected override async Task<ITransferResult> UpsertImpl()
        {
            await Task.Run(() =>
            {
                File.WriteAllBytes(Path, Value);
            });
            return TransferResult.Success;
        }

        protected override async Task<ITransferResult> DeleteImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) return TransferResult.NotFound;

                File.Delete(Path);
                return TransferResult.Found;
            }).ConfigureAwait(false);
        }

        protected override async ITask<IGetResult<byte[]>> GetImpl(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) { return RetrieveResult<byte[]>.NotFound; }

                return RetrieveResult<byte[]>.Success(OnRetrievedObject(File.ReadAllBytes(Path)));
            }).ConfigureAwait(false);
        }
    }
}
