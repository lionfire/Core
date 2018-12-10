using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.ObjectBus;
using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
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
            Root = reference;
        }

        public Mount(Vob vob, Vob target, string package = null, string store = null, bool enable = false, MountOptions mountOptions = null)
            : this(vob, target.GetHandle<MountHandleObject>(), package, store, enable, mountOptions)
        {
        }

        private Mount(Vob vob, H<MountHandleObject> rootHandle, string package = null, string store = null, bool enable = false, MountOptions mountOptions = null)
        {
            if (vob == null)
            {
                throw new ArgumentNullException($"{nameof(vob)}");
            }

            Vob = vob;
            VobDepth = vob.VobDepth;
            Root = rootHandle.Reference;
            this.rootHandle = rootHandle;
            Package = package;
            Store = store;
            //this.MountName =GetMountName(packageName, layerName);
            MountOptions = mountOptions;//.HasValue ? mountOptions.Value : MountOptions.Default;

            IsEnabled = enable; // Do this last, as it triggers a mount
        }

        #endregion

        internal readonly int VobDepth;

        public readonly Vob Vob;

        public readonly string Package;
        public readonly string Store;
        //public readonly string MountName;
        //string IKeyed<string>.Key { get { return MountName; } }

        //object IKeyed.Key { get { return this.Root.Key; } }
#if AOT
        string IROStringKeyed.Key { get { return this.Root.Key; } }
#else
        string IKeyed<string>.Key => Root.Key;
#endif


        public static string GetMountName(string packageName = null, string layerName = null)
        {
            var mountName = ((packageName ?? "") + VosPath.LocationDelimiter + (layerName ?? "")).Trim(VosPath.LocationDelimiter);
            return string.IsNullOrWhiteSpace(mountName) ? null : mountName;
        }

        /// <summary>
        /// MountHandleObject
        /// </summary>
        public RH<MountHandleObject> RootHandle
        {
            get
            {
                if (rootHandle == null)
                {
                    rootHandle = Root.GetReadHandle<MountHandleObject>();
                }
                return rootHandle;
            }
        }
        private RH<MountHandleObject> rootHandle;

        

        public readonly IReference Root;

        public readonly MountOptions MountOptions;

        #region State

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                {
                    return;
                }

                isEnabled = value;

                if (value)
                {
                    Vob.Mount(this);
                }
                else
                {
                    Vob.Unmount(this);
                }
            }
        }
        private bool isEnabled;

        #endregion

        public override string ToString() => "{Mount " + Vob + " ==> " + Root + "}";

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
