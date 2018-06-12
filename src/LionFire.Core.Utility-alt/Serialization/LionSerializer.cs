using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
//using System.Xml.Serialization;

namespace LionFire.Serialization
{

#if false // UNUSED
    public class EmptySerializer : LionSerializer
    {
        public override string Name
        {
            get { return "Empty"; }
        }

        public override byte[][] IdentifyingHeaders
        {
            get { return new byte[][] { new byte[] {}  }; }
        }

        public override T Deserialize<T>(Stream stream)
        {
            if (stream.Length == 0) return default(T);
            throw new LionSerializationException("EmptySerializer expected empty stream");
        }

        public override object Deserialize(Stream stream, Type type)
        {
            if (stream.Length == 0) return null;
            throw new LionSerializationException("EmptySerializer expected empty stream");
        }

        public override void Serialize(Stream stream, object graph)
        {
            if (graph != null) throw new ArgumentException("EmptySerializer can only serialize null");
        }

        public override string DefaultFileExtension
        {
            get { return "null"; }
        }
    }
#endif
    public interface ILionSerializerBase
    {
        void Serialize(Stream stream, object graph);
    }
    public abstract class LionSerializer : ILionSerializerBase
    {
        public abstract string Name { get; }
        public abstract byte[][] IdentifyingHeaders { get; }

        public bool MatchesHeader(Stream stream)
        {
            if (stream.Length == 0)
            {
                if (IdentifyingHeaders.Contains(new byte[] { })) return true;
            }

            int magicSize = 1;

            byte[] magic = new byte[magicSize];
            int bytesRead = stream.Read(magic, 0, magicSize);
            try
            {
                if (bytesRead < magicSize)
                {
                    return false;
                    //throw new Exception("File is too short (" + stream.Length + " bytes) to determine its serialization header.  It may be corrupt.");
                }

                List<byte[]> headers = new List<byte[]>(IdentifyingHeaders);
                List<byte[]> removals = new List<byte[]>();
                for (int i = 0; i < magicSize; i++)
                {
                    foreach (byte[] header in headers)
                    {
                        if (header.Length == 0) removals.Add(header);
                        if (i < header.Length)
                        {
                            if(magic[i] != header[i]) removals.Add(header);
                        }
                        
                    }
                    foreach (var header in removals)
                    {
                        headers.Remove(header);
                    }
                    removals.Clear();
                }

                return true;
            }
            finally
            {
                stream.Seek(-1 * bytesRead, SeekOrigin.Current);
            }
        }

        public abstract T Deserialize<T>(Stream stream);
        public abstract object Deserialize(Stream stream, Type type);


        public abstract void Serialize(Stream stream, object graph);

        public abstract string DefaultFileExtension { get; }
        public virtual string[] FileExtensions { get { return new string[] { DefaultFileExtension }; } }

        public string DotDefaultFileExtension { get { return "." + DefaultFileExtension; } }

        public virtual string GetDiskPath(string path, object obj, Type type)
        {
            // FUTURE: For multitype saving, consider injecting type into filename somehow.
            // FUTURE: For multilocation saving, determine actual save directory
            return path + DotDefaultFileExtension;
        }

        //public virtual string GetSerializePath(string path, object obj, Type type) NOT NEEDED YET
        //{
        //    if (path.EndsWith(FileDotExtension))
        //    {
        //        return path.Substring(0, path.Length - FileDotExtension.Length);
        //    }

        //    return path;
        //}
        
    }

    public static class LionSerializerExtensions
    {
        public static object Deserialize(this LionSerializer ser, byte[] bytes, Type type)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ser.Deserialize(ms, type);
            }
        }
        public static T Deserialize<T>(this LionSerializer ser, byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ser.Deserialize<T>(ms);
            }
        }
    }

    public interface IStringLionSerializer : ILionSerializerBase { }

    public static class StringLionSerializerExtensions
    {
        public static string Serialize<T>(this IStringLionSerializer ser, object obj)
        {
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, obj);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }
    }
}
