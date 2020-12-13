using LionFire.Persistence;
using LionFire.Resolves;
using MorseCode.ITask;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{

    public class HTextFile : HLocalFileBase<string>
    {
        #region Construction

        public HTextFile() { }
        public HTextFile(string path) : base(path)
        {
        }

        #endregion
         
        protected override async Task<IPersistenceResult> UpsertImpl()
        {
            await Task.Run(() =>
            {
                File.WriteAllText(Path, Value);
            }).ConfigureAwait(false);
            return PersistenceResult.Success;
        }

        protected override async Task<IPersistenceResult> DeleteImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) return PersistenceResult.NotFound;

                File.Delete(Path);
                return PersistenceResult.Success;
            }).ConfigureAwait(false);
        }

        protected override async ITask<IResolveResult<string>> ResolveImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) return RetrieveResult<string>.NotFound;

                return RetrieveResult<string>.Success(OnRetrievedObject(File.ReadAllText(Path)));
            }).ConfigureAwait(false);
        }
    }
}
