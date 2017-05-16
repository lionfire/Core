using System;
using System.Collections.Generic;
using System.Reflection;

namespace LionFire.Serialization
{
    public abstract class SerializerBase : ISerializer
    {

        public virtual IEnumerable<string> FileExtensions { get { yield break; } }
        public virtual IEnumerable<string> MimeTypes { get { yield break; } }

        #region DefaultSettingsFlags

        public SerializationFlags DefaultSettingsFlags
        {
            get { return defaultSettingsFlags; }
            set { if (defaultSettingsFlags == value) return; defaultSettingsFlags = value; OnFlagsChanged(); }
        }

        public abstract SerializationFlags SupportedCapabilities { get; }

        protected SerializationFlags defaultSettingsFlags;

        protected virtual void OnFlagsChanged()
        {
        }

        #endregion

        public virtual object DefaultDeserializationSettings { get; set; }
        public virtual object DefaultSerializationSettings { get; set; }

        #region Serialize

        public abstract byte[] ToBytes(object obj, SerializationContext context = null);

        public abstract string ToString(object obj, SerializationContext context = null);

        #endregion

        #region Deserialize

        public abstract T ToObject<T>(byte[] bytes, SerializationContext context = null);
        public abstract T ToObject<T>(string serializedData, SerializationContext context = null);

        // FUTURE if needed?
        //public virtual object ToObject(byte[] bytes, Type type)
        //{
        //    var obj = ToObject(bytes);
        //    if (!type.IsAssignableFrom(obj.GetType()))
        //    {
        //        throw new ArgumentException("Got type " + obj.GetType().FullName + " but exepected type matching parameter: " + type.FullName);
        //    }
        //    return obj;
        //}


        #endregion

    }
}
