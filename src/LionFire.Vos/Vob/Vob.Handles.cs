//#define SharedHandles // No longer use these, now that we have WeakHandleRegistry (it conflicts with attached Finalizers)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Persistence;
using LionFire.Vos.Mounts;
using LionFire.Referencing;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Vos.Services;
using System.Collections.Concurrent;

namespace LionFire.Vos
{

    public partial class Vob
    {
        #region Handles: Create and Get shared

        #region Read

#if SharedHandles
        private ConcurrentDictionary<Type, IReadHandle> SharedReadHandles
        {
            get { if (sharedReadHandles == null) sharedReadHandles = new ConcurrentDictionary<Type, IReadHandle>(); return sharedReadHandles; }
        }
        private ConcurrentDictionary<Type, IReadHandle> sharedReadHandles;

        public IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default)
            => (IReadHandle<T>)SharedReadHandles.GetOrAdd(typeof(T), t => CreateReadHandle<T>(preresolvedValue));
#else
        public IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default)
            //=> (IReadHandle<T>)SharedReadHandles.GetOrAdd(typeof(T), t => CreateReadHandle<T>(preresolvedValue));
            => CreateReadHandle<T>(preresolvedValue);
#endif

        public IReadHandle<T> CreateReadHandle<T>(T preresolvedValue = default)
            => new PersisterReadHandle<IVobReference, T, VosPersister>(this.GetRequiredService<VosPersister>(), VobReference.ForType<T>(), preresolvedValue);

        #endregion

        #region ReadWrite

#if SharedHandles

        private ConcurrentDictionary<Type, IReadWriteHandle> SharedReadWriteHandles
        {
            get { if (sharedReadWriteHandles == null) sharedReadWriteHandles = new ConcurrentDictionary<Type, IReadWriteHandle>(); return sharedReadWriteHandles; }
        }
        private ConcurrentDictionary<Type, IReadWriteHandle> sharedReadWriteHandles = new ConcurrentDictionary<Type, IReadWriteHandle>();

        public IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue preresolvedValue = default)
            => (IReadWriteHandle<TValue>)SharedReadWriteHandles.GetOrAdd(typeof(TValue), t => CreateReadWriteHandle<TValue>(preresolvedValue));
#else
        public IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue preresolvedValue = default)
                    => CreateReadWriteHandle<TValue>(preresolvedValue);
#endif

        public IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue preresolvedValue = default)
            => new PersisterReadWriteHandle<IVobReference, TValue, VosPersister>(this.GetService<VosPersister>(), GetReference<TValue>().ForType<TValue>(), preresolvedValue);

        #endregion

        #region Write

#if SharedHandles
        private ConcurrentDictionary<Type, IWriteHandle> SharedWriteHandles => sharedWriteHandles ??= new ConcurrentDictionary<Type, IWriteHandle>();
        private ConcurrentDictionary<Type, IWriteHandle> sharedWriteHandles;

        public IWriteHandle<TValue> GetWriteHandle<TValue>(TValue prestagedValue = default)
            => (IWriteHandle<TValue>)SharedWriteHandles.GetOrAdd(typeof(TValue), t => CreateWriteHandle<TValue>(prestagedValue));
#else
        public IWriteHandle<TValue> GetWriteHandle<TValue>(TValue prestagedValue = default)
         => CreateWriteHandle<TValue>(prestagedValue);
#endif
        //=> (IWriteHandle<T>)WriteHandles.GetOrAdd(typeof(T), t => new PersisterWriteHandle<VobReference, T, VosPersister>(this.GetService<VosPersister>(), VobReference));
        public IWriteHandle<T> CreateWriteHandle<T>(T prestagedValue = default)
            => new PersisterWriteHandle<IVobReference, T, VosPersister>(this.GetService<VosPersister>(), VobReference.ForType<T>(), prestagedValue);

        #endregion

        #region Collection

        #endregion

        #endregion

        //public IEnumerable<IReadHandle<MountHandleObject>> ReadHandles
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts())
        //        {
        //            yield break;
        //        }
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //            {
        //                yield return GetMountHandle(mount);
        //            }
        //        }
        //    }
        //}

        //private IEnumerable<KeyValuePair<H, Mount>> ReadHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<R, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }
        //    }
        //}

        public IEnumerable<Mount> ReadHandleMounts
        {
            get
            {
                l.Warn($"{nameof(ReadHandleMounts)} Not Implemented");
                yield break;
                //if (!InitializeEffectiveMounts())
                //{
                //    yield break;
                //}
                ////if (HasMounts)
                //{
                //    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                //    {
                //        yield return mount;
                //    }
                //}

            }
        }

        public bool CanWrite => WriteHandleMounts.Where(m => !m.Options.IsReadOnly()).Any();

        public IEnumerable<IReadHandleBase<MountHandleObject>> WriteHandles
        {
            get
            {
                l.Warn($"{nameof(WriteHandles)} Not Implemented");
                yield break;
                //NextVobNode.WriteMounts
                //if (!InitializeEffectiveMounts())
                //{
                //    yield break;
                //}

                ////if (!HasMounts) yield break;
                //foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                //{
                //    yield return GetMountHandle(mount);
                //}
            }
        }

        //private IEnumerable<KeyValuePair<H, Mount>> WriteHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<H, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }

        //    }
        //}
        private IEnumerable<Mount> WriteHandleMounts
        {
            get
            {
                l.Warn($"{nameof(WriteHandleMounts)} Not Implemented");
                yield break;
                //if (!InitializeEffectiveMounts())
                //{
                //    yield break;
                //}
                ////if (HasMounts)
                //{
                //    foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                //    {
                //        yield return mount;
                //    }
                //}

            }
        }

        private IEnumerable<Mount> EffectiveWriteMounts
        {
            get
            {
                l.Warn($"{nameof(EffectiveWriteMounts)} Not Implemented");
                yield break;
                //if (effectiveMountsByWritePriority == null) { InitializeEffectiveMounts(); }
                //if (effectiveMountsByWritePriority == null) { return Enumerable.Empty<Mount>(); }
                //return effectiveMountsByWritePriority.Values.SelectMany(x => x);
            }
        }
        private IReadHandleBase<MountHandleObject> FirstWriteHandle // REVIEW Don't use this?
        {
            get
            {
                l.Warn($"{nameof(FirstWriteHandle)} Not Implemented");
                return null;
                //                //if (!HasMounts) return null;
                //                foreach (Mount mount in
                //#if AOT
                //                        (IEnumerable)
                //#endif
                // EffectiveWriteMounts)
                //                {
                //                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
                //                    {
                //                        continue;
                //                    }

                //                    return GetMountHandle(mount);
                //                }
                //                return null;
            }
        }
        private IReadWriteHandleBase<T> GetFirstWriteHandle<T>()
        {
            throw new NotImplementedException();
            //            //get
            //            {
            //                //if (!HasMounts) return null;
            //                foreach (Mount mount in
            //#if AOT
            //                        (IEnumerable)
            //#endif
            // EffectiveWriteMounts)
            //                {
            //                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
            //                    {
            //                        continue;
            //                    }

            //                    return GetReadWriteHandleFromMount<T>(mount);
            //                }
            //                return null;
            //            }
        }

        //private VobHandle<T> GetFirstWriteHandle<T>()
        //{
        //    H objectHandle;
        //    objectHandle = FirstWriteHandle<T>();
        //    return objectHandle;
        //}

        private IReadHandleBase<MountHandleObject> GetFirstWriteHandle()
        {
            throw new NotImplementedException();
            //IReadHandleBase<MountHandleObject> objectHandle;

            ////if (package == null && layer == null)
            //{
            //    objectHandle = FirstWriteHandle;
            //    return objectHandle;
            //}
            ////            else
            ////            {
            ////                if (package != null && layer != null)
            ////                {
            ////                    string mountName = LionFire.Vos.Mount.GetMountName(package, layer);
            ////                    Mount mount = effectiveMountsByName.TryGetValue(mountName);
            ////                    if (mount.MountOptions.IsReadOnly)
            ////                    {
            ////                        l.Trace("GetFirstWriteHandle found mount by name but it IsReadOnly");
            ////                        return null;
            ////                    }
            ////                    else
            ////                    {
            ////                        return GetMountHandle(mount);
            ////                    }
            ////                }
            ////                else
            ////                {
            ////                    if (package != null)
            ////                    {
            ////                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.PackageName == package).FirstOrDefault());
            ////                    }
            ////                    else // layer != null
            ////                    {
            ////#if SanityChecks
            ////                        if (layer == null) throw new UnreachableCodeException("layer != null");
            ////#endif
            ////                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.LayerName == layer).FirstOrDefault());
            ////                    }
            ////                }
            ////            }
        }

    }
}
