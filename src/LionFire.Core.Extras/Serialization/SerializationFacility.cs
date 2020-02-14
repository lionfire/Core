using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using LionFire.Collections;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace LionFire.Serialization
{

    public static class LegacySerializationFacility // TODO, Rename to file store, integrate with existing ObjectStore/FileStore????
    {

        #region Serializers

        // Move to LionSerializers somehow?

        public static LionSerializer DefaultSerializer = LionSerializers.Json;
        public static LionSerializer ReadableSerializer = LionSerializers.Json;

        public static LionSerializer CompactSerializer =
#if MSGPACK
         LionSerializers.LionPack;
#else
         LionSerializers.Json;
#endif

        private static LionSerializer GetSerializer(SerializationParameters parameters = null)
        {
            if (parameters == null) return DefaultSerializer;

            if (parameters.SaveInArchive) throw new NotImplementedException();

            if (parameters.SaveReadable)
            {
                return ReadableSerializer;
            }
            else
            {
                return CompactSerializer;
            }
        }

        #endregion

        #region Serialize

        public static void Serialize<T>(string path, T obj, SerializationParameters parameters = null)
        {
            LionSerializer serializer = GetSerializer(parameters);

            string diskPath = serializer.GetDiskPath(path, obj, typeof(T));

            string diskDirectory = Path.GetDirectoryName(diskPath);

            if (!Directory.Exists(diskDirectory))
            {
                Directory.CreateDirectory(diskDirectory);
            }

            using (FileStream fs = new FileStream(diskPath, FileMode.Create))
            {
                serializer.Serialize(fs, obj);
            }
        }

        #endregion

        #region Extensions

        public static string DotDefaultExtension
        {
            get { return Serializers.First().DotDefaultFileExtension; }
        }

        #endregion

        #region Exists

        public static string GetSerializationPathForPath(string path)
        {
            if (File.Exists(path)) return path;
            else
            {
                foreach (LionSerializer prospectiveSerializer in
                    #if AOT
                        (IEnumerable)
#endif
                    Serializers)
                {
                    string dotExtension = prospectiveSerializer.DotDefaultFileExtension;
                    string pathPlusExtension = path + dotExtension;

                    if (File.Exists(pathPlusExtension))
                    {
                        return pathPlusExtension;
                    }
                }
            }
            return null;
        }

        public static bool Exists(string path)
        {
            var serializationPath = GetSerializationPathForPath(path);
            return serializationPath != null;
        }

        public static void Delete(string path)
        {
            var serializationPath = GetSerializationPathForPath(path);
            if (serializationPath != null)
            {
                File.Delete(serializationPath);
            }
#if OLD
            if (File.Exists(path)) { File.Delete(path); return ; }
            else
            {
                foreach (LionSerializer prospectiveSerializer in Serializers)
                {
                    string dotExtension = prospectiveSerializer.DotDefaultFileExtension;
                    string pathPlusExtension = path + dotExtension;

                    if (File.Exists(pathPlusExtension))
                    {
                        File.Delete(pathPlusExtension);
                        return ;
                    }
                }
            }
            return ;
#endif
        }

        public static bool Exists<T>(string path)
        {
            if (Exists(path)) return true;

            foreach (LionSerializer prospectiveSerializer in Serializers)
            {
                string dotExtension = prospectiveSerializer.DotDefaultFileExtension;
                string pathPlusTypePlusExtension = path + "." + typeof(T).Name + dotExtension;

                if (File.Exists(pathPlusTypePlusExtension))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Deserialization

        #region By Stream

        public static T Deserialize<T>(Stream stream)
        {
            try
            {
                LionSerializer serializer = null;
                serializer = LionSerializers.DetectSerializer(stream);

                if (serializer == null) throw new Exception("Could not detect deserialization method.");

                return serializer.Deserialize<T>(stream);
            }
            catch (Exception ex)
            {
                l.Error("Deserialization failed: " + ex.ToString());
                throw;
            }
        }
        public static object Deserialize(Stream stream, Type type, string path = null)
        {
            try
            {
                LionSerializer serializer = null;

                serializer = LionSerializers.DetectSerializer(stream, path);

                if (serializer == null) throw new Exception("Could not detect deserialization method.");

                return serializer.Deserialize(stream, type);
            }
            catch (Exception ex)
            {
                l.Error("Deserialization of '"+path+"' failed: " + ex.ToString());
                throw;
            }
        }

        #endregion

        #region By Path

        public static object Deserialize(string path, Type type = null)
        {
            try
            {
                return Deserialize<object>(path);
            }
            catch (Exception)
            {
                l.Error("Failed to deserialize file: " + path);
                throw;
            }
            //using (FileStream stream = new FileStream(path, FileMode.Open))
            //{
            //    LionSerializer serializer = null;
            //    serializer = DetectSerializer(stream);

            //    if (serializer == null) throw new Exception("Could not detect deserialization method.");

            //    return serializer.Deserialize(stream, type);
            //}
        }
                

		public static T Deserialize<T>(string path, LionSerializer serializer = null)
			where T : class
		{
			object result = Deserialize(path, serializer, typeof(T));
			return (T) result;
		}
		[AotReplacement]
		public static object Deserialize(string path, LionSerializer serializer, Type T)
		{
            object result = null;

            if (File.Exists(path))
            {
                int retryCount = 100;
                int retryDelayMilliseconds = 10;
            retry:
                try
                {
                    //using (FileStream stream = new FileStream(path, FileMode.Open))
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    //using (StreamReader stream = new StreamReader(File.OpenRead(path)))
                    {
                        if (serializer == null)
                        {
                            serializer = LionSerializers.DetectSerializer(stream);
                        }
                        if (serializer == null) throw new Exception("Could not recognize file.");

                        result = serializer.Deserialize (stream, T);
                        goto gotResult;
                    }
                }
                catch (IOException ioe)
                {
                    l.Debug("Got IOException when opening file.  It may be busy: " + ioe.ToString());

                    if (retryCount > 0)
                    {
                        Thread.Sleep(retryDelayMilliseconds);
                        retryCount--;
                        goto retry;
                    }
                    else
                    {
                        throw ioe;
                    }
                }

            }
            else
            {
                if (serializer != null) throw new ArgumentException("Either path must exist or serializer must be null.");

                // FUTURE: Support saving multiple types at the same name (multitype/mixin style behavior)

                foreach (LionSerializer prospectiveSerializer in
#if AOT
 (IEnumerable)
#endif
                    Serializers)
                {
                    string dotExtension = prospectiveSerializer.DotDefaultFileExtension;
                    string pathPlusExtension = path + dotExtension;

                    if (File.Exists(pathPlusExtension))
                    {
                        result = Deserialize(pathPlusExtension, prospectiveSerializer, T);
                        goto gotResult;
                    }
                }

                foreach (LionSerializer prospectiveSerializer in
#if AOT
 (IEnumerable)
#endif
                    Serializers)
                {
                    string dotExtension = prospectiveSerializer.DotDefaultFileExtension;
                    string pathPlusTypePlusExtension = path + "." + (T).Name + dotExtension;

                    if (File.Exists(pathPlusTypePlusExtension))
                    {
                        result = Deserialize(pathPlusTypePlusExtension, prospectiveSerializer, T);
                        goto gotResult;
                    }
                }
            }
            throw new SerializationException(SerializationOperationType.FromStream, message: "Failed to find file: " + path);

        gotResult:
            INotifyDeserialized notifyDeserialized = result as INotifyDeserialized;
            if (notifyDeserialized != null) notifyDeserialized.OnDeserialized();

            return result;
        }

        #endregion

        #endregion

        public static IReadOnlyCollection<LionSerializer> Serializers { get { return LionSerializers.Serializers; } }


        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
