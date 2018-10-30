using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public class VBase : OBase<VosReference>
    {

        #region Ontology

        #region Root

        public Vob Root
        {
            get
            {
                return root;
            }
        }
        private readonly RootVob root;

        #endregion

        public override IOBus OBus => VosOBus.Instance;

        #endregion


        #region Scheme

        public override IEnumerable<string> UriSchemes => VosReference.UriSchemes;

        #endregion

        #region Handles

        // TODO
        public override H<T> GetHandle<T>(IReference reference) => throw new NotImplementedException();
        
        #endregion
        
        #region Get

        public override Task<IRetrieveResult<T>> TryGet<T>(VosReference reference)
        {
            var result = new RetrieveResult<T>();
            // TODO
            try
            {
                if (reference.Type != null)
                {
                    if (!reference.Type.IsAssignableFrom(type))
                    {
                        throw new ArgumentException("!reference.Type.IsAssignableFrom(typeof(ResultType))");
                    }
                }

                Vob vob = Root.GetChild(reference);
                if (vob == null) return null;

                var vh = vob.ToHandle(type);

                if (optionalRef != null) { vh.IsRetrieveInfoEnabled = true; }
                if (vh.TryEnsureRetrieved())
                {
                    result.Result = vh.Object;
                }
                else
                {
                    // Or weak references auto cleanup?? - this was QueryChild, but that doesn't allow for querying in non-loaded locations!  It might be nice to somehow query for a Vob with an object then clean up that part of the Vob tree if it doesn't need to exist anymore.
                    //vob.Cleanup();
                    return null;
                }
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }

            return result;
        }

        public override Task<IRetrieveResult<object>> TryGet(VosReference reference, Type type)
        {
            return TryGet<object>(reference);
        }

        #endregion

        #region Set

        protected override Task _Set(VosReference reference, object obj, Type type = null, bool allowOverwrite = true, bool preview = false) => throw new NotImplementedException();

        #endregion

        #region Delete

        public override Task<bool?> CanDelete(VosReference reference) => throw new NotImplementedException();
        public override Task<bool> TryDelete(VosReference reference, bool preview = false) => throw new NotImplementedException();

        #endregion

        #region Children

        public override IEnumerable<string> GetChildrenNames(VosReference parent) => throw new NotImplementedException();
        public override IEnumerable<string> GetChildrenNamesOfType<T>(VosReference parent) => throw new NotImplementedException();

        #endregion

    }
}
