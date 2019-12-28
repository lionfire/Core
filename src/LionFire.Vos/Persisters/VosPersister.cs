using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters.Vos
{
    public class VosPersister :
        PersisterBase<VosPersisterOptions>
        , IPersister<VosReference>
    //, IFilesystemPersistence<TReference, TPersistenceOptions>
    //, IWriter<string> // FUTURE?
    //, IReader<string> // FUTURE?
    {

        #region RootName

        [SetOnce]
        public string RootName
        {
            get => rootName;
            set
            {
                if (rootName == value) return;
                if (rootName != default) throw new AlreadySetException();
                rootName = value;
            }
        }
        private string rootName;

        #endregion

        #region Root

        public Vob Root
        {
            get { return root; }
        }
        private Vob root;

        #endregion

        public VosPersister(VosRootManager vosRootManager, string rootName, VosPersisterOptions options)
        {
            this.RootName = rootName;
            root = vosRootManager.Get(rootName);
        }

        public Task<IPersistenceResult> Exists(IReferencable<VosReference> referencable) => throw new System.NotImplementedException();

        public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<VosReference> referencable)
        {
            var vob = root[referencable.Reference.Path];

            var result = new VosRetrieveResult<TValue>();

            bool anyMounts = false;
            foreach(var mount in vob.GetNext<VobMounts>().RankedEffectiveReadMounts)
            {
                //var relativeDepth = vob.Depth - mount.VobDepth;

                var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);

                var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);

                var rh = effectiveReference.GetReadHandle<TValue>();

                //anyMounts = true;
                //var rh = vob.GetReadHandleFromMount<TValue>(mount);
                //if (rh == null) continue;

                //var childResult = await rh.Retrieve().ConfigureAwait(false);
                var childResult = (await rh.Resolve().ConfigureAwait(false)).ToRetrieveResult();

                if (childResult.IsFail()) result.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    result.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister failed

                    if (childResult.Flags.HasFlag(PersistenceResultFlags.Found))
                    {
                        result.Value = childResult.Value;
                        return childResult;
                    }
                }
            }
            if (!anyMounts) result.Flags |= PersistenceResultFlags.MountNotAvailable;
            return result;
        }

        public Task<IPersistenceResult> Create<TValue>(IReferencable<VosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Update<TValue>(IReferencable<VosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<VosReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Delete(IReferencable<VosReference> referencable) => throw new System.NotImplementedException();
    }
}
