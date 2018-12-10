using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{
    //public static class IReferenceableTreeHandleExtensions
    //{
    //    public static H<T1> GetHandle<T1>(this IReferencable r, params string[] subpathChunks) where T1 : class => r.Reference.GetChildSubpath(subpathChunks).GetHandle<T1>();
    //    public static H<T1> GetHandle<T1>(this IReferencable r, IEnumerable<string> subpathChunks) where T1 : class => r.Reference.GetChildSubpath(subpathChunks).GetHandle<T1>();
    //}

   // OPTIMIZE - (Here and VobHandle)  Create Reference on demand?  Store the reference path in Vob, instead of VosReference?  Generate VosReference on demand?  or is this an app specific thing?

    [ReadOnlyEditionFor(typeof(VobHandle<>))]
    public class VobReadHandle<T> : RBase<T>
        , IVobReadHandle<T> // Has T because it is contravariantt
        , IVobHandle // No T because it is invariant
        , IProvidesHandleFromSubPath
      //, IHandleProvider -- FUTURE: get relative paths? Maybe a stretch.
    {
        public virtual bool IsReadOnly => true;
        
        #region Ontology

        public new VosReference Reference
        {
            get => (VosReference)base.Reference;
            set => base.Reference = value;
        }

        #endregion

        private static VBase VBase => InjectionContext.Current.GetService<VosOBus>().DefaultVBase;
        public Type Type => Reference.Type ?? typeof(T);
        public string Path => Reference.Path;

        #region Vob

        IVob IVobHandle.Vob => Vob;
        IVob IVobReadHandle<T>.Vob => Vob;
        public Vob Vob
        {
            get { return vob; }
            set
            {
                if (vob == value) return;
                if (vob != default(Vob)) throw new AlreadySetException();
                vob = value;
                Reference = vob.GetReference<T>();
            }
        }
        private Vob vob;

        #endregion

        #region Construction

        public VobReadHandle(Vob vob)
        {
            this.Vob = vob;
        }

        /// <summary>
        /// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        /// </summary>
        /// <param name="vosReference"></param>
        public VobReadHandle(VosReference vosReference)
        {
            SetFromVosReference(vosReference);
        }

        /// <summary>
        /// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        /// </summary>
        /// <param name="reference">Currently must be of type VosReference.  (FUTURE: Allow reference types compatible with / convertible to VosReference)</param>
        public VobReadHandle(IReference reference) : this((VosReference)reference)
        {
            SetFromVosReference(reference.ToVosReference());
        }

        private void SetFromVosReference(VosReference vosReference)
        {
            vosReference.ValidateReference<T>();
            // TODO: Verify host and any sort of Vos root tree id??
            Vob = VBase[vosReference.Path];
        }

        #endregion

        #region IProvidesHandleFromSubPath

        public H<THandle> GetHandleFromSubPath<THandle>(params string[] subpathChunks) => Vob[subpathChunks].GetHandle<THandle>();
        public H<THandle> GetHandleFromSubPath<THandle>(IEnumerable<string> subpathChunks) => Vob[subpathChunks].GetHandle<THandle>();
        public RH<THandle> GetReadHandleFromSubPath<THandle>(params string[] subpathChunks) => Vob[subpathChunks].GetReadHandle<THandle>();
        public RH<THandle> GetReadHandleFromSubPath<THandle>(IEnumerable<string> subpathChunks) => Vob[subpathChunks].GetReadHandle<THandle>();

        //// Move this to IReferencable extensions?

        //public H<T1> GetHandle<T1>(params string[] subpathChunks) where T1 : class => Reference.GetChildSubpath(subpathChunks).GetHandle<T1>();
        //public H<T1> GetHandle<T1>(IEnumerable<string> subpathChunks) where T1 : class => Reference.GetChildSubpath(subpathChunks).GetHandle<T1>();

        //// REVIEW - string, vs params string[] - does that resolve the way I think?
        //public H this[string subpath] => Reference.GetChildSubpath(subpath).GetHandle();
        //public H this[IEnumerable<string> subpathChunks] => Reference.GetChildSubpath(subpathChunks).GetHandle();
        //public H this[params string[] subpathChunks] => Reference.GetChildSubpath(subpathChunks).GetHandle();
        //public H this[int index, string[] subpathChunks] => throw new NotImplementedException();

        #endregion

        #region Read handle implementation

        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();

        #endregion

        #region Resolution info

        public Mount Mount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public VosRetrieveInfo VosRetrieveInfo => throw new NotImplementedException();

        //public string Store { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string Package { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Settings

        //public bool AutoLoad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Object

        //object IVobHandle.Object { get => Object; set => Object = (T)value; }
        //object H<object>.Object { get => Object; set => Object = (T)value; }
        //object IReadWrapper<object>.Object => Object;
        //object IWriteWrapper<object>.Object { set => Object = (T)value; }

        //public bool IsObjectReferenceFrozen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public event PropertyChangedEventHandler ObjectPropertyChanged;

        #endregion

        #region Merging

        //public void MergeWith(IVobHandle existing) => throw new NotImplementedException();
        //public void MergeInto(IVobHandle mainHandle) => throw new NotImplementedException();

        #endregion

        //public void Unload() => throw new NotImplementedException();

        //public IVobHandle GetSibling(string name) => throw new NotImplementedException();
        //public bool CanWriteToReadSource() => throw new NotImplementedException();

    }
}
