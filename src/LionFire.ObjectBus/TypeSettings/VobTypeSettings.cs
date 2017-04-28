using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

    public enum VobTypeFlags
    { 
        None = 0,

        /// <summary>
        /// For a single Vob path, there can be zero or more decorators present, but only 
        /// a max of one non-decorator
        /// </summary>
        Decorator = 1 << 1,
    }

    public class VobTypeSettings
    {
        #region Type

        public Type Type
        {
            get { return type; }
            set
            {
                if (type == value) return;
                if (type != default(Type)) throw new AlreadySetException();
                type = value;
            }
        } private Type type;

        #endregion

        private void InitForType(Type type)
        {
            if (typeof(IKeyed).IsAssignableFrom(type))
            {
                GetKey = GetKeyFromROKeyed;
            }

            if(type.HasCustomAttribute<DecoratorAttribute>())
            {
                VobTypeFlags |= ObjectBus.VobTypeFlags.Decorator;
            }
            
        }

        #region Key

        public Func<object, object> GetKey { get; set; }

        public static object GetKeyFromROKeyed(object vobObject)
        {
            return ((IKeyed)vobObject).Key;
        }

        #endregion


        #region VobTypeFlags

        public VobTypeFlags VobTypeFlags
        {
            get { return vobTypeFlags; }
            set { vobTypeFlags = value; }
        } private VobTypeFlags vobTypeFlags;

        #endregion



    }

}
