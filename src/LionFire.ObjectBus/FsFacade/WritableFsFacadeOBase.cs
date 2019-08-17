using LionFire.ObjectBus.FsFacade;
using LionFire.Persistence;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public abstract class WritableFsFacadeOBase<TReference> : WritableOBase<TReference>, IConnectingOBase
        where TReference : class, IReference
    {
        public abstract IFsFacade FsFacade { get; }
        public abstract string ConnectionString { get; set; }

        #region Write

        public override async Task<(bool exists, IPersistenceResult result)> Exists(TReference reference)
        {
            var result = new RetrieveResult<bool>();

            bool existsResult = await FsFacade.Exists(reference.Path).ConfigureAwait(false);

            result.Flags |= PersistenceResultFlags.Success | (existsResult ? PersistenceResultFlags.Found : PersistenceResultFlags.NotFound);
            return (existsResult, result);
        }

        public override async Task<IPersistenceResult> CanDelete<T>(TReference reference)
        {
            // FUTURE: Check filesystem permissions
            var existsResult = await Exists(reference);
            return existsResult.exists ? PersistenceResult.PreviewSuccess : PersistenceResult.NotFound;

            //return existsResult.Object;
            //return new RetrieveResult<bool?>
            //{
            //    IsSuccess = existsResult.IsSuccess,
            //    Result = existsResult.Result,
            //};
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override async Task<IPersistenceResult> TryDelete<T>(TReference reference)
        {
            string filePath = reference.Path;
            //if (!defaultTypeForDirIsT)
            //{
            //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
            //}

            //if (preview)
            //{
            //    return await FsFacade.Exists(filePath).ConfigureAwait(false);
            //}
            //else
            //{
            return await FsFacade.Delete(filePath).ConfigureAwait(false);

            //}
        }

        #endregion

    }

}
