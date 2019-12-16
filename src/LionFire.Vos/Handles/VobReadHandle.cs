using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Copying;
using LionFire.Dependencies;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Vos
{
    //public static class IReferenceableTreeHandleExtensions
    //{
    //    public static H<T1> ToHandle<T1>(this IReferencable r, params string[] subpathChunks) where T1 : class => r.Reference.GetChildSubpath(subpathChunks).ToHandle<T1>();
    //    public static H<T1> ToHandle<T1>(this IReferencable r, IEnumerable<string> subpathChunks) where T1 : class => r.Reference.GetChildSubpath(subpathChunks).ToHandle<T1>();
    //}

   // OPTIMIZE - (Here and VobHandle)  Create Reference on demand?  Store the reference path in Vob, instead of VosReference?  Generate VosReference on demand?  or is this an app specific thing?

    [ReadOnlyEditionFor(typeof(VobHandle<>))]
    public class VobReadHandle<T> : ReadHandleBase<IVosReference, T>
        , IVobReadHandle<T> // Has T because it is contravariantt
        , ISubpathHandleProvider
      //, IReadWriteHandleProvider -- FUTURE: get relative paths? Maybe a stretch.
    {
        public virtual bool IsReadOnly => true;
        
        #region Ontology

        public new IVosReference Reference
        {
            get => (VosReference)base.Reference;
            set => base.Reference = value;
        }

        #endregion

        //private static VBase VBase => DependencyContext.Current.GetService<VosOBus>().DefaultVBase;
        public Type Type => Reference.Type ?? typeof(T);
        public string Path => Reference.Path;

        #region Vob

        //IVob IVobHandle<object>.Vob => Vob;
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

        ///// <summary>
        ///// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        ///// </summary>
        ///// <param name="vosReference"></param>
        //public VobReadHandle(VosReference vosReference)
        //{
        //    SetFromVosReference(vosReference);
        //}

        ///// <summary>
        ///// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        ///// </summary>
        ///// <param name="reference">Currently must be of type VosReference.  (FUTURE: Allow reference types compatible with / convertible to VosReference)</param>
        //public VobReadHandle(IReference reference) : this((VosReference)reference)
        //{
        //    SetFromVosReference(reference.ToVosReference());
        //}

        //private void SetFromVosReference(VosReference vosReference)
        //{
        //    vosReference.ValidateReference<T>();
        //    // TODO: Verify host and any sort of Vos root tree id??
        //    Vob = VBase[vosReference.Path];
        //}

        #endregion

        #region IProvidesHandleFromSubPath

        public IReadHandleBase<THandle> GetReadHandleFromSubPath<THandle>(params string[] subpathChunks) => Vob[subpathChunks].GetReadHandle<THandle>();
        public IReadHandleBase<THandle> GetReadHandleFromSubPath<THandle>(IEnumerable<string> subpathChunks) => Vob[subpathChunks].GetReadHandle<THandle>();
        

        public IReadWriteHandleBase<THandle> GetReadWriteHandleFromSubPath<THandle>(params string[] subpathChunks) => Vob[subpathChunks].GetHandle<THandle>();
        public IReadWriteHandleBase<THandle> GetReadWriteHandleFromSubPath<THandle>(IEnumerable<string> subpathChunks) => Vob[subpathChunks].GetHandle<THandle>();
        
        //public IWriteHandleBase<THandle> GetWriteHandleFromSubPath<THandle>(IEnumerable<string> subpathChunks) => Vob[subpathChunks].GetWriteHandle<THandle>();

        //// Move this to IReferencable extensions?

        //public H<T1> ToHandle<T1>(params string[] subpathChunks) where T1 : class => Reference.GetChildSubpath(subpathChunks).ToHandle<T1>();
        //public H<T1> ToHandle<T1>(IEnumerable<string> subpathChunks) where T1 : class => Reference.GetChildSubpath(subpathChunks).ToHandle<T1>();

        //// REVIEW - string, vs params string[] - does that resolve the way I think?
        //public H this[string subpath] => Reference.GetChildSubpath(subpath).ToHandle();
        //public H this[IEnumerable<string> subpathChunks] => Reference.GetChildSubpath(subpathChunks).ToHandle();
        //public H this[params string[] subpathChunks] => Reference.GetChildSubpath(subpathChunks).ToHandle();
        //public H this[int index, string[] subpathChunks] => throw new NotImplementedException();

        #endregion

        #region Read handle implementation

        public void OnRenamed(IVobHandle<object> newHandle) => throw new NotImplementedException();
        protected override ITask<IResolveResult<T>> ResolveImpl() => throw new NotImplementedException();

        #endregion

        #region Mount

        protected IReference MountReference {
            get {
                var path = MountPath;
                if (path == null) return null;
                return new VosReference(path);
            }
        }

        public string MountPath {
            get {
                if (Mount == null || Vob == null) return null;
                var result = Vob.GetMountPath(Mount);
                return result;
            }
        }

        [Assignment(AssignmentMode.Assign)]
        public Mount Mount {
            get => mount;
            set {
                if (mount == value) return;
                if (value != null && mount != default(Mount)) throw new NotSupportedException("Mount can only be set once, unless it is set back to null.");
                mount = value;
            }
        }
        private Mount mount;

        #endregion

        #region Package

        public string EffectivePackage {
            get {
                if (Mount != null && Mount.Package != null) return Mount.Package;
                return Package;
            }
        }
        public string Package {
            get {
                //if (Mount != null && Mount.Package != null) return Mount.Package;
                return package;
            }
            set {
                if (package == value) return;
                if (package != default(string)) throw new NotSupportedException("Package can only be set once.");
                package = value;
            }
        }
        private string package;

        #endregion

        #region Store

        public string EffectiveStore {
            get {
                if (Mount != null && Mount.Store != null) return Mount.Store;
                return Store;
            }
        }
        public string Store {
            get {
                if (Mount != null && Mount.Store != null) return Mount.Store;
                return store;
            }
            set {
                if (store == value) return;
                if (store != default(string)) throw new NotSupportedException("Store can only be set once.");
                store = value;
            }
        }
        private string store;

        #endregion

        #region Resolution info

        //public VosRetrieveInfo VosRetrieveInfo => throw new NotImplementedException();

        //public string Store { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string Package { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Settings

        //public bool AutoLoad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Object

        //object IVobHandle<object>.Object { get => Object; set => Object = (T)value; }

        //object IReadWrapper<object>.Object => throw new NotImplementedException();
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


        //event Action<RH<object>, HandleEvents> RH<object>.HandleEvents
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //event Action<RH<object>, object, object> RH<object>.ObjectReferenceChanged
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //event Action<RH<object>> RH<object>.ObjectChanged
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


    }
}
