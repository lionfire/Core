using LionFire.Serialization.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Serialization
{
    public abstract class SerializerBase : ISerializerStrategy
    {

        #region FileExtensions

        public virtual IEnumerable<string> FileExtensions { get { yield break; } }

        public virtual string DefaultFileExtension
        {
            get
            {
                return defaultFileExtension ?? FileExtensions.First();
            }
            set
            {
                defaultFileExtension = value;
            }
        }
        protected string defaultFileExtension;

        #endregion

        #region MimeTypes

        public virtual IEnumerable<string> MimeTypes { get { yield break; } }


        public virtual string DefaultMimeType
        {
            get
            {
                return defaultMimeType ?? MimeTypes.First();
            }
            set
            {
                defaultMimeType = value;
            }
        }
        protected string defaultMimeType;

        #endregion
        
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

        public virtual float GetPriorityForContext(SerializationContext context)
        {
            float score = 0;

            var fs = context as FileSerializationContext; // Move to derived class?
            if (fs != null)
            {
                if (FileExtensions.Contains(fs.FileExtension)) score += 100;
            }

            return score;
        }

        #region Serialize

        public abstract byte[] ToBytes(SerializationContext context);

        public abstract string ToString(SerializationContext context);

        #endregion

        #region Deserialize

        public abstract T ToObject<T>(SerializationContext context);

        //public abstract T ToObject<T>(string serializedData, SerializationContext context = null);

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
