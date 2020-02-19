using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters.Vos
{
    public class VosPersister :
        PersisterBase<VosPersisterOptions>
        , IPersister<IVosReference>
    //, IFilesystemPersistence<TReference, TPersistenceOptions>
    //, IWriter<string> // FUTURE?
    //, IReader<string> // FUTURE?
    {

        //#region RootName

        //[SetOnce]
        //public string RootName
        //{
        //    get => rootName;
        //    set
        //    {
        //        if (rootName == value) return;
        //        if (rootName != default) throw new AlreadySetException();
        //        rootName = value;
        //    }
        //}
        //private string rootName;

        //#endregion

        #region Root

        public IRootVob Root { get; private set; }

        #endregion

        IServiceProvider ServiceProvider => Root?.GetServiceProvider();

        public VosPersister(IRootVob root)
        {
            Root = root;
        }
        public VosPersister(IRootManager vosRootManager, string rootName, VosPersisterOptions options) : this(vosRootManager.Get(rootName))
        {
            //this.RootName = rootName;
            //root = vosRootManager.Get(rootName);
        }


        public Task<IPersistenceResult> Exists<TValue>(IReferencable<IVosReference> referencable) => throw new System.NotImplementedException();

        public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IVosReference> referencable)
        {
            var vob = Root[referencable.Reference.Path];

            var result = new VosRetrieveResult<TValue>();

            bool anyMounts = false;
            var vobMounts = vob.AcquireNext<VobMounts>();
            if (vobMounts != null)
            {
                foreach (var mount in vobMounts.RankedEffectiveReadMounts)
                {

                    //var relativeDepth = vob.Depth - mount.VobDepth;

                    var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);

                    var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);
                    
                    var rh = effectiveReference.GetReadHandle<TValue>(ServiceProvider);

                    //anyMounts = true;
                    //var rh = vob.GetReadHandleFromMount<TValue>(mount);
                    //if (rh == null) continue;

                    //var childResult = await rh.Retrieve().ConfigureAwait(false);
                    var childResult = (await rh.Resolve().ConfigureAwait(false)).ToRetrieveResult();

                    if (childResult.IsFail()) result.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

                    if (childResult.IsSuccess == true)
                    {
                        result.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister succeeded

                        if (childResult.Flags.HasFlag(PersistenceResultFlags.Found))
                        {
                            result.Value = childResult.Value;
                            return childResult;
                        }
                    }
                }
            }
            if (!anyMounts) result.Flags |= PersistenceResultFlags.MountNotAvailable;
            return result;
        }

        public Task<IPersistenceResult> Create<TValue>(IReferencable<IVosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Update<TValue>(IReferencable<IVosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<IVosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Delete(IReferencable<IVosReference> referencable) => throw new System.NotImplementedException();
        public Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<IVosReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
    }
}
