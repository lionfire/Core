#nullable enable
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystemlike
{

    public static class FilesystemlikePersisterExtensions
    {
        public static async Task<ITransferResult> DeleteResultToPersistenceResult(this Task<bool?> deleteTask)
        {
            var deleteResult = await deleteTask.ConfigureAwait(false);
            if (!deleteResult.HasValue) return TransferResult.Indeterminate;
            return deleteResult.Value ? TransferResult.SuccessAndFound : TransferResult.SuccessNotFound;
        }
    }

    // REVIEW - is there a way to do this?
    //public static class PersisterBaseExtensions
    //{
    //    public static Task<IGetResult<TValue>> Retrieve<TValue, TReference>(this IPersister<TReference> persister, TReference reference)
    //        => persister.Retrieve<TValue>((TReference)reference.Path);
    //}

    #region ENH - RecentSaves
#if RecentSaves
        // REVIEW - what is the point of this?
        // TODO: Move this to some sort of monitoring service

        public ConcurrentDictionary<string, DateTime> RecentSaves => recentSaves;
        private readonly ConcurrentDictionary<string, DateTime> recentSaves = new ConcurrentDictionary<string, DateTime>();

        public void CleanRecentSaves()
        {
            foreach (var kvp in RecentSaves)
            {
                if (DateTime.UtcNow - kvp.Value > TimeSpan.FromMinutes(3))
                {
                    RecentSaves.TryRemove(kvp.Key, out DateTime dt);
                }
            }
        }
#endif
    #endregion

    #region Scraps

    //public class Pipeline
    //{
    //    protected List<Action<Pipeline>> pipeline;
    //    public void Next();
    //}

    //public delegate object RetrieveProcessor(object x);

    //public interface IFilesystemWritePersister
    //{
    //    Stream CreateWriteStream(string fsPath, ReplaceMode replaceMode);
    //    Task BytesToPath(string fsPath, byte[] bytes, ReplaceMode replaceMode);
    //    Task StringToPath(string fsPath, string str, ReplaceMode replaceMode);

    //    //Func<TValue, string, ISerializationStrategy, ReplaceMode, PersistenceOperation, PersistenceContext, SerializationResult> SerializeToString<TValue>(TValue obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context) { get;}

    //    // Task<SerializationResult> SerializeToBytes<TValue>(TValue obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context)

    //    //Task<SerializationResult> SerializeToStream<TValue>(TValue obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context)
    //}

    #endregion
}
