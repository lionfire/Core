using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public class VBase : OBase<VosReference>, IVBase
    {
        #region Ontology

        #region Root

        public Vob Root => root;
        private readonly RootVob root;

        #endregion

        public override IOBus OBus => VosOBus.Instance;

        #endregion

        #region Construction

        public VBase()
        {
            root = new RootVob(this);
        }

        #endregion

        #region Scheme

        public override IEnumerable<string> UriSchemes => VosReference.UriSchemes;

        #endregion

        #region Handles

        public override H<T> GetHandle<T>(IReference reference) => new VobHandle<T>(reference);
        public override RH<T> GetReadHandle<T>(IReference reference) => new VobReadHandle<T>(this[reference.Path]);

        #endregion

        #region Get

        /// <summary>
        /// For Vos, the main get logic is in the VobHandles&lt;T&gt;, so this is largely a pass-through to that
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public override async Task<IRetrieveResult<T>> TryGet<T>(VosReference reference)
        {
            var result = new RetrieveResult<T>();

            try
            {
                var vh = reference.GetReadHandle<T>();

                //vh.IsRetrieveInfoEnabled = true;  TOPORT ?
                if (await vh.TryGetObject().ConfigureAwait(false))
                {
                    result.Object = vh.Object;
                }
                else
                {
                    // Clean up weak references if nothing found?? - this was QueryChild, but that doesn't allow for querying in non-loaded locations!  It might be nice to somehow query for a Vob with an object then clean up that part of the Vob tree if it doesn't need to exist anymore.
                    //vob.Cleanup();
                    return null;
                }
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex); // FIXME - move this to VobHandle
                throw ex;
            }

            return result;
        }

        public override Task<IRetrieveResult<object>> TryGet(VosReference reference, Type type) => TryGet<object>(reference);

        #endregion

        #region Set

        protected override Task _Set(VosReference reference, object obj, Type type = null, bool allowOverwrite = true, bool preview = false) => throw new NotImplementedException();

        #endregion

        #region Delete

        public override Task<bool?> CanDelete(VosReference reference) => throw new NotImplementedException();
        public override Task<bool?> TryDelete(VosReference reference) => throw new NotImplementedException();

        #endregion

        #region List

        public override Task<IEnumerable<string>> GetKeys(VosReference parent) => throw new NotImplementedException();
        public override Task<IEnumerable<string>> GetKeysOfType<T>(VosReference parent) => throw new NotImplementedException();
        public override IEnumerable<string> GetChildrenNames(VosReference parent) => throw new NotImplementedException();
        public override IEnumerable<string> GetChildrenNamesOfType<T>(VosReference parent) => throw new NotImplementedException();

        #endregion


        #region (Public) Vos-specific

        #region Vos-style Accessors

        public Vob this[string path] => Root[path];
        public Vob this[VosReference reference] => Root[reference.Path]; // TODO: Verify VosReference refers to this VBase

        #endregion

        #endregion

        #region (Internal) Vos implementation

        #region Vos Mounts

        // RECENTCHANGE - was SynchronizedObservableCollection
        internal MultiBindableCollection<Vob> VobsWithMounts
        {
            get
            {
                return vobsWithMounts;
            }
        }
        private MultiBindableCollection<Vob> vobsWithMounts = new MultiBindableCollection<Vob>();

        internal void OnMountsChangedFor(Vob changedVob, INotifyCollectionChangedEventArgs<Mount> e)
        {
            foreach (var vobWithMounts in VobsWithMounts.ToArray())
            {
                if (changedVob.IsAncestorOf(vobWithMounts))
                {
                    vobWithMounts.OnAncestorMountsChanged(changedVob, e);
                }
            }
        }

        #endregion

        #endregion

        
    }
}
