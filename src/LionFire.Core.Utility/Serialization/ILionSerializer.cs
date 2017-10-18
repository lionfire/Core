using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    /// <summary>
    ///  New generation serializer framework
    /// </summary>
    /// 

    public interface ILionSerializer
    {

        /// <summary>
        /// Returns true if the flags are supported by this serializer
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        bool SupportsCapability(SerlializationSettingsFlags flags);

        bool ImplicitDetect(byte[] bytes);
        bool ExplicitDetect(byte[] bytes);
        byte[] ExplicitIdentificationMarker { get; }

        byte[] GetBytes(object obj);
        object ToObject(byte[] bytes);
        T ToObject<T>(byte[] bytes);
    }

    public abstract class LionSerializerBase : ILionSerializer
    {

        #region Flags

        public SerlializationSettingsFlags Flags
        {
            get { return flags; }
            set { if (flags == value) return; flags = value; OnFlagsChanged(); }
        } protected SerlializationSettingsFlags flags;

        protected virtual void OnFlagsChanged()
        {

        }

        #endregion

        public bool SupportsCapability(SerlializationSettingsFlags flags)
        {
            return false;
        }

        public bool ImplicitDetect(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public bool ExplicitDetect(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] ExplicitIdentificationMarker
        {
            get { throw new NotImplementedException(); }
        }

        // TODO: GetString?

        #region GetBytes

        public abstract byte[] GetBytes(object obj);

        #endregion

        #region ToObject

        public abstract object ToObject(byte[] bytes);
        public virtual T ToObject<T>(byte[] bytes)
        {
            return (T)ToObject(bytes);
        }
        public virtual object ToObject(byte[] bytes, Type type)
        {
            var obj = ToObject(bytes);
            if (!type.IsAssignableFrom(obj.GetType()))
            {
                throw new ArgumentException("Got type " + obj.GetType().FullName + " but exepected type matching parameter: " + type.FullName);
            }
            return obj;
        }

        #endregion

    }
}
