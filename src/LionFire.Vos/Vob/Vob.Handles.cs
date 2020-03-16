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

        private ConcurrentDictionary<Type, IReadHandle> SharedReadHandles
        {
            get { if (sharedReadHandles == null) sharedReadHandles = new ConcurrentDictionary<Type, IReadHandle>(); return sharedReadHandles; }
        }
        private ConcurrentDictionary<Type, IReadHandle> sharedReadHandles;

        public IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default)
            => (IReadHandle<T>)SharedReadHandles.GetOrAdd(typeof(T), t => CreateReadHandle<T>(preresolvedValue));

        public IReadHandle<T> CreateReadHandle<T>(T preresolvedValue = default)
            => new PersisterReadHandle<VosReference, T, VosPersister>(this.GetRequiredService<VosPersister>(), VosReference, preresolvedValue);

        #endregion

        #region ReadWrite

        private ConcurrentDictionary<Type, IReadWriteHandle> SharedReadWriteHandles
        {
            get { if (sharedReadWriteHandles == null) sharedReadWriteHandles = new ConcurrentDictionary<Type, IReadWriteHandle>(); return sharedReadWriteHandles; }
        }
        private ConcurrentDictionary<Type, IReadWriteHandle> sharedReadWriteHandles = new ConcurrentDictionary<Type, IReadWriteHandle>();

        public IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue preresolvedValue = default)
            => (IReadWriteHandle<TValue>)SharedReadWriteHandles.GetOrAdd(typeof(TValue), t => CreateReadWriteHandle<TValue>(preresolvedValue));

        public IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue preresolvedValue = default)
            => new PersisterReadWriteHandle<VosReference, TValue, VosPersister>(this.GetService<VosPersister>(), VosReference, preresolvedValue);

        #endregion

        #region Write

        private ConcurrentDictionary<Type, IWriteHandle> SharedWriteHandles => sharedWriteHandles ??= new ConcurrentDictionary<Type, IWriteHandle>();
        private ConcurrentDictionary<Type, IWriteHandle> sharedWriteHandles;

        public IWriteHandle<TValue> GetWriteHandle<TValue>(TValue prestagedValue = default)
            => (IWriteHandle<TValue>)SharedWriteHandles.GetOrAdd(typeof(TValue), t => CreateReadWriteHandle<TValue>(prestagedValue));
        
        //=> (IWriteHandle<T>)WriteHandles.GetOrAdd(typeof(T), t => new PersisterWriteHandle<VosReference, T, VosPersister>(this.GetService<VosPersister>(), VosReference));
        public IWriteHandle<T> CreateWriteHandle<T>(T prestagedValue = default)
            => new PersisterWriteHandle<VosReference, T, VosPersister>(this.GetService<VosPersister>(), VosReference, prestagedValue);

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
                throw new NotImplementedException();
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
                //NextVobNode.WriteMounts
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
                //if (effectiveMountsByWritePriority == null) { InitializeEffectiveMounts(); }
                //if (effectiveMountsByWritePriority == null) { return Enumerable.Empty<Mount>(); }
                //return effectiveMountsByWritePriority.Values.SelectMany(x => x);
            }
        }
        private IReadHandleBase<MountHandleObject> FirstWriteHandle // REVIEW Don't use this?
        {
            get
            {
                throw new NotImplementedException();
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
