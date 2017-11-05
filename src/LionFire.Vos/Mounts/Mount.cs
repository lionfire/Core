using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Structures;

namespace LionFire.ObjectBus
{
    public class Mount :
#if AOT
        IROStringKeyed
#else
		IKeyed<string>
#endif
    {
        #region Construction

        public Mount(Vob vob, IReference reference, string package = null, string store = null, bool enable = false, MountOptions mountOptions = null)
            : this(vob, reference.GetHandle<MountHandleObject>(), package, store, enable, mountOptions)
        {
            this.Root = reference;
        }

        public Mount(Vob vob, Vob target, string package = null, string store = null, bool enable = false, MountOptions mountOptions = null)
            : this(vob, target.ToHandle<MountHandleObject>(), package, store, enable, mountOptions)
        {
        }

        private Mount(Vob vob, IHandle rootHandle, string package = null, string store = null, bool enable = false, MountOptions mountOptions = null)
        {
            if (vob == null) throw new ArgumentNullException("vob");
            this.Vob = vob;
            this.VobDepth = vob.VobDepth;
            this.Root = rootHandle.Reference;
            this.rootHandle = rootHandle;
            this.Package = package;
            this.Store = store;
            //this.MountName =GetMountName(packageName, layerName);
            this.MountOptions = mountOptions;//.HasValue ? mountOptions.Value : MountOptions.Default;

            this.IsEnabled = enable; // Do this last, as it triggers a mount
        }

        #endregion

        internal readonly int VobDepth;

        public readonly Vob Vob;

        public readonly string Package;
        public readonly string Store;
        //public readonly string MountName;
        //string IKeyed<string>.Key { get { return MountName; } }

        object IKeyed.Key { get { return this.Root.Key; } }
#if AOT
        string IROStringKeyed.Key { get { return this.Root.Key; } }
#else
        string IKeyed<string>.Key { get { return this.Root.Key; } }
#endif


        public static string GetMountName(string packageName = null, string layerName = null)
        {
            var mountName = ((packageName ?? "") + LionPath.LocationDelimiter + (layerName ?? "")).Trim(LionPath.LocationDelimiter);
            return StringX.IsNullOrWhiteSpace(mountName) ? null : mountName;
        }

        public IHandle RootHandle {
            get {
                if (rootHandle == null)
                {
                    rootHandle = Root.GetHandle<MountHandleObject>();
                }
                return rootHandle;
            }
        }
        private IHandle rootHandle;

        public class MountHandleObject : INotifyOnSaving
        {
            public void OnSaving()
            {
                throw new NotSupportedException("Do not save this object");
            }
        }

        public readonly IReference Root;

        public readonly MountOptions MountOptions;

        #region State

        public bool IsEnabled {
            get {
                return isEnabled;
            }
            set {
                if (isEnabled == value) return;

                isEnabled = value;

                if (value)
                {
                    this.Vob.Mount(this);
                }
                else
                {
                    this.Vob.Unmount(this);
                }
            }
        }
        private bool isEnabled;

        #endregion

        public override string ToString()
        {
            return "{Mount " + Vob + " ==> " + Root + "}";
        }

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }
}
