using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LionFire.Persistence;

namespace LionFire.Serialization
{

    public abstract class SerializerBase<ConcreteType> : ISerializationStrategy
        where ConcreteType : SerializerBase<ConcreteType>
    {
        public virtual bool SupportsSerializedType(Type type) => true;
        public virtual bool SupportsDeserializedType(Type type) => true;

        #region SerializerBaseReflectionInfo

        private static class SerializerBaseReflectionInfo<T>
            where T : SerializerBase<T>, ISerializationStrategy
        {
            public static readonly bool HasToString;
            public static readonly bool HasToBytes;
            public static readonly bool HasToStream;

            public static readonly bool HasFromString;
            public static readonly bool HasFromBytes;
            public static readonly bool HasFromStream;

            static SerializerBaseReflectionInfo()
            {
                //foreach (var mi in typeof(T).GetMethods())
                //{
                //    Debug.WriteLine(mi.Name + " ");
                //    foreach (var param in mi.GetParameters())
                //    {
                //        Debug.WriteLine(" - " + param.ParameterType.Name);
                //    }
                //}

                HasToString = typeof(T).GetMethod("ToString", new Type[] { typeof(object), typeof(Lazy<PersistenceOperation>), typeof(PersistenceContext) }).DeclaringType != typeof(SerializerBase<T>);
                HasToBytes = typeof(T).GetMethod("ToBytes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).DeclaringType != typeof(SerializerBase<T>);
                HasToStream = typeof(T).GetMethod("ToStream", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).DeclaringType != typeof(SerializerBase<T>);

                HasFromString = typeof(T).GetMethod("ToObject", new Type[] { typeof(string), typeof(Lazy<PersistenceOperation>), typeof(PersistenceContext) }).DeclaringType != typeof(SerializerBase<T>);
                HasFromBytes = typeof(T).GetMethod("ToObject", new Type[] { typeof(byte[]), typeof(Lazy<PersistenceOperation>), typeof(PersistenceContext) }).DeclaringType != typeof(SerializerBase<T>);
                HasFromStream = typeof(T).GetMethod("ToObject", new Type[] { typeof(Stream), typeof(Lazy<PersistenceOperation>), typeof(PersistenceContext) }).DeclaringType != typeof(SerializerBase<T>);
            }
        }

        #endregion

        public virtual IEnumerable<ISerializeScorer> SerializeScorers => Enumerable.Empty<ISerializeScorer>();
        public virtual IEnumerable<IDeserializeScorer> DeserializeScorers => Enumerable.Empty<IDeserializeScorer>();


        public virtual IEnumerable<SerializationFormat> Formats { get { yield return DefaultFormat; } }
        public abstract SerializationFormat DefaultFormat { get; }

        #region DefaultSettingsFlags

        public SerializationFlags DefaultSettingsFlags
        {
            get => defaultSettingsFlags;
            set { if (defaultSettingsFlags == value) { return; } defaultSettingsFlags = value; OnFlagsChanged(); }
        }

        public abstract SerializationFlags SupportedCapabilities { get; }

        protected SerializationFlags defaultSettingsFlags;

        protected virtual void OnFlagsChanged()
        {
        }

        #endregion

        public virtual IEnumerable<Type> SerializationOptionsTypes => Enumerable.Empty<Type>();

        public bool ImplementsToString => SerializerBaseReflectionInfo<ConcreteType>.HasToString;
        public bool ImplementsToStream => SerializerBaseReflectionInfo<ConcreteType>.HasToStream;
        public bool ImplementsToBytes => SerializerBaseReflectionInfo<ConcreteType>.HasToBytes;
        public bool ImplementsFromString => SerializerBaseReflectionInfo<ConcreteType>.HasFromString;
        public bool ImplementsFromStream => SerializerBaseReflectionInfo<ConcreteType>.HasFromStream;
        public bool ImplementsFromBytes => SerializerBaseReflectionInfo<ConcreteType>.HasFromBytes;

        #region Serialize

        public virtual (byte[] Bytes, SerializationResult Result) ToBytes(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (ImplementsToString)
            {
                var (String, Result) = ToString(obj, operation, context);
                return (Result.IsSuccess ? StringToBytes(String, context) : null, Result);
            }
            else if (ImplementsToStream)
            {
                using (var ms = new MemoryStream())
                {
                    var result = ToStream(obj, ms, operation, context);
                    if (!result.IsSuccess) { return (null, result); }
                    ms.Seek(0, SeekOrigin.Begin);
                    return (ms.StreamToBytes(), result);
                }
            }
            return (null, SerializationResult.NotSupported);
        }

        public virtual (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (ImplementsToBytes)
            {
                var (Bytes, Result) = ToBytes(obj, operation, context);
                return (Result.IsSuccess ? BytesToString(Bytes, context) : null, Result);
            }
            else if (ImplementsToStream)
            {
                using (var ms = new MemoryStream())
                {
                    var result = ToStream(obj, ms, operation, context);
                    if (!result.IsSuccess) { return (null, result); }
                    ms.Seek(0, SeekOrigin.Begin);
                    return (BytesToString(ms.StreamToBytes(), context), result);
                }
            }
            return (null, SerializationResult.NotSupported);
        }

        public virtual SerializationResult ToStream(object obj, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            byte[] bytes;
            SerializationResult result;
            if (ImplementsToBytes)
            {
                (bytes, result) = ToBytes(obj, operation, context);
                if (!result.IsSuccess) { return result; }
            }
            else if (ImplementsToStream)
            {
                string str;
                (str, result) = ToString(obj, operation, context);
                if (!result.IsSuccess) { return result; }
                bytes = StringToBytes(str, context);
            }
            else
            {
                return SerializationResult.NotSupported;
            }
            stream.Write(bytes, 0, bytes.Length);
            return SerializationResult.Success;
        }

        #endregion

        #region Deserialize

        public virtual DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (SerializerBaseReflectionInfo<ConcreteType>.HasFromBytes)
            {
                return ToObject<T>(StringToBytes(str, context), operation, context);
            }
            return DeserializationResult<T>.NotSupported;
        }

        public virtual DeserializationResult<T> ToObject<T>(byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (SerializerBaseReflectionInfo<ConcreteType>.HasFromString)
            {
                return ToObject<T>(BytesToString(bytes, context), operation, context);
            }
            return DeserializationResult<T>.NotSupported;
        }

        public virtual DeserializationResult<T> ToObject<T>(Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var bytes = new byte[stream.Length - stream.Position];
            stream.Read(bytes, 0, bytes.Length);

            if (SerializerBaseReflectionInfo<ConcreteType>.HasToBytes)
            {
                return ToObject<T>(bytes, operation, context);
            }
            else if (SerializerBaseReflectionInfo<ConcreteType>.HasToString)
            {
                return ToObject<T>(BytesToString(bytes, context), operation, context);
            }
            else
            {
                return DeserializationResult<T>.NotSupported;
            }
        }

        public virtual Encoding DefaultEncoding => System.Text.UTF8Encoding.UTF8;
        protected byte[] StringToBytes(string str, PersistenceContext context = null) => str == null ? null : (context?.SerializationContext?.Encoding ?? DefaultEncoding).GetBytes(str);
        protected string BytesToString(byte[] bytes, PersistenceContext context = null) => bytes == null ? null : (context?.SerializationContext?.Encoding ?? DefaultEncoding).GetString(bytes);

        //public abstract T ToObject<T>(string serializedData, SerializationContext context = null);

        #endregion

        #region OLD / Resurrect / MOVE

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

        //#region FileExtensions

        //public virtual IEnumerable<string> FileExtensions { get { yield break; } }

        //public virtual string DefaultFileExtension
        //{
        //    get => defaultFileExtension ?? FileExtensions.First();
        //    set => defaultFileExtension = value;
        //}
        //protected string defaultFileExtension;

        //#endregion

        //#region MimeTypes

        //public virtual IEnumerable<string> MimeTypes { get { yield break; } }


        //public virtual string DefaultMimeType
        //{
        //    get => defaultMimeType ?? MimeTypes.First();
        //    set => defaultMimeType = value;
        //}
        //protected string defaultMimeType;

        //#endregion

        //public virtual object DefaultDeserializationSettings => null;
        //public virtual object DefaultSerializationSettings => null;

        //public virtual float GetPriorityForContext(SerializationContext context) // MOVE to standalone scoring strategy
        //{
        //    float score = 0;

        //    var fs = context as FileSerializationContext; // Move to derived class?
        //    if (fs != null)
        //    {
        //        if (FileExtensions.Contains(fs.FileExtension))
        //        {
        //            score += 100;
        //        }
        //    }

        //    return score;
        //}

        #endregion

    }

    public static class SerializationModeConversion
    {
        public static byte[] StreamToBytes(this Stream str)
        {
            byte[] bytes = new byte[str.Length];
            str.Read(bytes, 0, (int)str.Length);
            return bytes;
        }
        public static void BytesToStream(this byte[] bytes, Stream str) => str.Write(bytes, 0, bytes.Length);

    }
}
