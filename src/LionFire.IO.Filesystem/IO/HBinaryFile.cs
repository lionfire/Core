using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
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

        protected override async Task<IPersistenceResult> UpsertImpl()
        {
            await Task.Run(() =>
            {
                File.WriteAllBytes(Path, Value);
            });
            return PersistenceResult.Success;
        }

        protected override async Task<IPersistenceResult> DeleteImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) return PersistenceResult.NotFound;

                File.Delete(Path);
                return PersistenceResult.Found;
            }).ConfigureAwait(false);
        }

        protected override async ITask<IGetResult<byte[]>> ResolveImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) { return RetrieveResult<byte[]>.NotFound; }

                return RetrieveResult<byte[]>.Success(OnRetrievedObject(File.ReadAllBytes(Path)));
            }).ConfigureAwait(false);
        }
    }
}
