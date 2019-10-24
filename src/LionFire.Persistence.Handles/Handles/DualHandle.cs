#if false
using LionFire.Events;
using LionFire.Referencing;
using LionFire.Resolves;
using System;

namespace LionFire.Persistence
{
    /// <summary>
    /// TODO: Scrap this, and extend HandleBase to hold last known Value from data store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DualHandle<T> : IDualHandle<T>, IDisposable
    {

        #region Reference

        [SetOnce]
        public IReference Reference
        {
            get => reference;
            set
            {
                if (reference == value) return;
                if (reference != default) throw new AlreadySetException();
                reference = value;
            }
        }
        private IReference reference;

        #endregion

        #region Construction

        public DualHandle() { }
        public DualHandle(IReference reference) { }

        #endregion

        public H<T> LocalHandle
        {
            get
            {
                if (localHandle == null)
                {
                    if (Reference != null)
                    {
                        localHandle = reference.GetHandle<T>();
                        // TODO: If localHandle.Value is retrieved from data source, clone the value and set to remoteHandle.Value
                        if (remoteHandle != null) { CloneRemoteValueToLocal(); }
                    }
                }
                return localHandle;
            }
        }
        private H<T> localHandle;

        public RH<T> RemoteHandle
        {
            get
            {
                if (remoteHandle == null)
                {
                    if (Reference != null)
                    {
                        remoteHandle = reference.GetReadHandle<T>();
                        if (remoteHandle is INotifiesValueChanged<T> nvc)
                        {
                            nvc.ValueChanged += OnRemoteValueChanged;
                        }
                    }
                }
                return remoteHandle;
            }
            private set
            {
                if (value == null)
                {
                    if (remoteHandle != null)
                    {
                        nvc.ValueChanged -= Nvc_ValueChanged;
                    }
                }
                remoteHandle = value;
            }
        }
        private RH<T> remoteHandle;

        private void OnRemoteValueChanged(ValueChanged<T> obj)
        {
            if (localHandle != null && !localHandle.HasValue) CloneRemoteValueToLocal();
        }

        private void CloneRemoteValueToLocal()
        {
            LocalHandle.Value = RemoteHandle.Value;
        }

        public bool GetLocalValueFromRemote => throw new NotImplementedException();

        public void Revert()
        {
            var localHandleCopy = localHandle;
            if (localHandle is ILazilyResolves<T> lr)
            {
                lr.DiscardValue();
            }
        }

        public void Dispose() => RemoteHandle = null;
    }

}


#endif