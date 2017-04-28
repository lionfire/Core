using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    //// FUTURE: Lightweight vos reference classes?
    //public class VosPathReference : IReference
    //{
    //}


    public class VHReference : VosReference
    {

        #region Construction

        public VHReference() { }
        public VHReference(Type type) { this.Type = type; }
        public VHReference(Type type, string path) : base(path) { this.Type = type; }
        public VHReference(Type type, params string[] pathComponents) : base(pathComponents) { this.Type = type; }
        public VHReference(Type type, IVobHandle vobHandle)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (vobHandle.Type != type) throw new ArgumentException("vobHandle.Type must equal type parameter");
            this.Type = type;
            this.handle = vobHandle;
        }

        #endregion

        #region Type

        //public Type Type
        //{
        //    get { return type; }
        //    set
        //    {
        //        base.Type
        //        if (type == value) return;
        //        if (type != default(Type)) throw new AlreadySetException();
        //        type = value;
        //    }
        //} private Type type;

        #endregion

        public IVobHandle Handle
        {
            get
            {
                if (handle == null)
                {
                    if (Type == null)
                    {
                        throw new ArgumentNullException("Type");
                    }
                    handle = this.Vob.GetHandle(Type);
                }
                return handle;
            }
        }
        private IVobHandle handle;
    }

    
    public class VHReference<T> : VosReference
        where T : class
    {
        #region Construction

        public VHReference() { }
        public VHReference(string path) : base(path) { }
        public VHReference(params string[] pathComponents) : base(pathComponents) { }

        public VHReference(VobHandle<T> vh) { this.handle = vh; }

        public static implicit operator T(VHReference<T> me)
        {
            return me.Handle.Object;
        }
        public static implicit operator VHReference<T>(string vosPath)
        {
            return new VHReference<T>(vosPath);
        }
        public static implicit operator VHReference<T>(string[] vosPath)
        {
            return new VHReference<T>(vosPath);
        }

        #region To/From VobHandle<>
        
        public static implicit operator VHReference<T>(VobHandle<T> vh)
        {
            return new VHReference<T>(vh);
        }

        public static implicit operator VobHandle<T>(VHReference<T> vh)
        {
            return vh.Handle;
        }

        #endregion

        #endregion

        #region (Public) Properties

        public VobHandle<T> Handle
        {
            get
            {
                if (handle == null)
                {
                    handle = this.Vob.GetHandle<T>();
                }
                return handle;
            }
        }
        private VobHandle<T> handle;

        #endregion

        #region Derived Properties - Object

        public T Object
        {
            get { return Handle == null ? null : Handle.Object; }
        }

        #endregion

        #region VobHandle Pass-thru

        public bool Exists
        {
            get { return this.Handle.Exists; }
        }

        public VosReference VosReference { get { return this; } }

        #endregion

    }

}
