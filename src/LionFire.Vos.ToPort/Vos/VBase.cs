using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LionFire.Collections;
using LionFire.ObjectBus;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    // !!! PORT: Already partially ported

    /// <summary>
    /// Represents a Vos hierarchy, analogous to a file system.
    /// Typically only one exists per app, although it could be useful to create child Vos's as a sort of chroot, or as a simulation of a remote Vos.
    /// </summary>
    public class VBase : OBase<VosReference>, IVBase
    {
        #region (Static) Singleton

        public static VBase Default => ManualSingleton<VBase>.GuaranteedInstance;

        #endregion

        #region Persistence

        //public virtual object TryGet(IVobHandle handle) OLD - was trying to get resolvedto
        //{
        //    Vob vob = Root.GetChild(handle.reference);
        //    if (vob == null) return null;

        //    if (vob.TryEnsureRetrieved())
        //    {
        //        return vob.Object;
        //    }
        //    else
        //    {
        //        // Or weak references auto cleanup?? - this was QueryChild, but that doesn't allow for querying in non-loaded locations!  It might be nice to somehow query for a Vob with an object then clean up that part of the Vob tree if it doesn't need to exist anymore.
        //        //vob.Cleanup();
        //        return null;
        //    }
        //}

        public Type DefaultHandleTypeForConcreteType(Type type)
        {
            // FUTURE: Implement this: e.g. go to the base class or interface marked [VosType]
            return type;
        }

        // FUTURE: Should there be generic versions of these?
        public override void Set(VosReference reference, object obj, bool allowOverwrite = true, bool preview = false)
        {
            Vob vob = Root[reference];
            Type handleType = reference.Type ?? (obj == null ? null : DefaultHandleTypeForConcreteType(obj.GetType())) ?? typeof(object);
#if VosUnitype
            IVobHandle vh = vob.UnitypeHandle;
#else
            // TODO: Get VH for the type for the object
            IVobHandle vh = vob.ToHandle(handleType);
#endif

            if (vh == null || !vh.Type.IsAssignableFrom(handleType))
            {
                vh = vob.ToHandle(handleType);
            }

            if (!allowOverwrite && vh.Value != null) // RETRIEVE
            {
                throw new CannotOverwriteException("This would overwrite and overwrite is disabled");
            }
            vh.Value = obj;
            vh.Save(allowDelete: true, preview: preview);
        }

#region Delete

        //private bool? AggregateTernary(IEnumerable<bool?> values)
        //{
        //    Vob vob = Root[reference];
        //    bool? result = false;
        //    bool gotFail = false;
        //    foreach (var vh in vob.Handles.OfType<IVobHandle>())
        //    {
        //        if (vh.Object == null) continue;

        //        if (vh.Mount.MountOptions.IsReadOnly)
        //        {
        //            gotFail = true;
        //        }
        //        else
        //        {
        //            if (gotFail) { result = null; }
        //            else { result = true; }
        //        }
        //    }
        //    return result;
        //}
        public bool CanWrite(VosReference reference)
        {
            var vh = reference.ToHandle() as IVobHandle;
            return vh.CanWriteToReadSource();

            //bool? result =
            //Vob vob = Root[reference];
            //bool? result = false;
            //bool cantDeleteSomething = false;
            //foreach (var vh in vob.Handles.OfType<IVobHandle>())
            //{
            //    if (vh.Object == null) continue;

            //    if (vh.Mount.MountOptions.IsReadOnly)
            //    {
            //        cantDeleteSomething = true;
            //    }
            //    else
            //    {
            //        if (cantDeleteSomething) { result = null; }
            //        else { result = true; }
            //    }
            //}

            //return result;
        }

        /// <param name="reference"></param>
        /// <returns>true if can be fully deleted, false if no delete can take place, and null if it can be partially deleted</returns>
        public override bool? CanDelete(VosReference reference)
        {
            // ENH idea: block out an object by creating a "deleted" placeholder in a writable mount that has a higher read priority
            return CanWrite(reference);
        }
        // bool? IOBase.CanDelete(IReference reference)
        //{
        //    if (reference == null) return false;
        //    VosReference vr = reference as VosReference;
        //    if (vr == null) throw new ArgumentException("reference must be of type VosReference");
        //    return CanDelete(vr);
        //}

        public override bool TryDelete(VosReference reference, bool preview = false)
        {
            object obj = TryGet(reference);
            Set(reference, null, true, preview);
            return obj != null;
        }

#endregion


#endregion

#region Get Children Names

        public override IEnumerable<string> GetChildrenNames(VosReference parent)
        {
            return GetChildrenNames(parent, persistedOnly: true);

        }
        public IEnumerable<string> GetChildrenNames(VosReference parent, bool persistedOnly = true)
        {
            //var vob = this.Root.QueryChild(parent);
            var vob = this.Root.GetChild(parent);
            if (vob == null) return null;
            return vob.GetChildrenNames(persistedOnly: persistedOnly);
        }

        public override IEnumerable<string> GetChildrenNamesOfType<T>(VosReference parent)
        {
            if (parent == null) throw new ArgumentNullException("(VosReference) parent");
            //var vob = this.Root.QueryChild(parent);
            var vob = this.Root.GetChild(parent);
            if (vob == null) return null;
            return vob.GetChildrenNamesOfType<T>();
        }

#endregion

#region Root Handle Pass-through

#region Path Accessors
        
        public Vob this[params string[] pathChunks] {
            get {
                return this.Root[0, pathChunks];
            }
        }

        public Vob this[IEnumerable<string> pathChunks] {
            get {
                return this.Root[pathChunks];
            }
        }
        public Vob this[IEnumerator<string> pathChunks] {
            get {
                return this.Root[pathChunks];
            }
        }

#endregion

#region Handles

        public override IHandle<T> GetHandle<T>(IReference reference)
        {
            VosReference vr = (VosReference)reference;
            // Uses HandleFactory
            return reference.GetHandle<T>();
        }

        //public override IHandle<TValue> GetHandleSubpath<TValue>(IHandle handle, params string[] subpathChunks) 
        //{

        //    IVobHandle vh = (IVobHandle) handle;
        //    handle.Reference.GetChildSubpath(subpathChunks);
        //    return reference.ToHandle<TValue>();
        //}

#endregion

#endregion

#region Exception Policies

        public bool ThrowOnNoSaveLocation = true;

        internal void OnNoSaveLocation(Vob vob)
        {
            if (ThrowOnNoSaveLocation)
            {
                throw new VosException("No save location exists for " + vob.Path);
            }
        }

#endregion

        //protected override IVobHandle ConvertToHandleType(IHandle handle)
        //{
        //    if (handle == null) return null;
        //    IVobHandle reft = handle as IVobHandle;
        //    if (reft == null)
        //    {
        //        VosReference reference = handle.Reference as VosReference;
        //        if (reference != null)
        //        {
        //            return new VobHandle(reference);
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Unsupported Handle/Reference type: " + handle.GetType().FullName);
        //        }
        //    }
        //    return reft;
        //}

        private static readonly ILogger l = Log.Get();

    }
}
