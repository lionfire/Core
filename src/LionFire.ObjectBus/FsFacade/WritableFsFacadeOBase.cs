//#if TODO // Redo, looking at FSPersister
using LionFire.Persistence;
using LionFire.Persistence.FilesystemFacade;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

    public abstract class WritableFsFacadeOBase<TReference> : WritableOBase<TReference>, IConnectingOBase
        where TReference : class, IReference
    {
        public abstract ISimpleFilesystemFacade FsFacade { get; }
        public abstract string ConnectionString { get; set; }

        #region Write

        public override async Task<(bool exists, ITransferResult result)> Exists(TReference reference)
        {
            //var result = new RetrieveResult<bool>();

            bool existsResult = await FsFacade.Exists(reference.Path).ConfigureAwait(false);

            return existsResult ? (true, RetrieveResult<object>.Found()) : (false, RetrieveResult<object>.NotFound);

            //result.Flags |= TransferResultFlags.Success | (existsResult ? TransferResultFlags.Found : TransferResultFlags.NotFound);
            //return (existsResult, result);
        }

        /// <summary>
        /// Return PreviewSuccess and Found if the item exists, otherwise PreviewFail and NotFound
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public override async Task<ITransferResult> CanDelete<T>(TReference reference)
        {
            var existsResult = await Exists(reference);
            // FUTURE: Check filesystem permissions
            return new TransferResult { Flags = (existsResult.result.Flags.HasFlag(TransferResultFlags.Found) ? TransferResultFlags.PreviewSuccess | TransferResultFlags.Found : TransferResultFlags.PreviewFail | TransferResultFlags.NotFound) }; 

            //return existsResult.Object;
            //return new RetrieveResult<bool?>
            //{
            //    IsSuccess = existsResult.IsSuccess,
            //    Result = existsResult.Result,
            //};
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override async Task<ITransferResult> TryDelete<T>(TReference reference)
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
//#endif