using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Serialization;
using System.IO;
using LionFire.Types;
using LionFire;
using LionFire.Structures;

namespace LionFire.ObjectBus
{
    public class OBusSerialization
    {
        public static OBusSerialization Instance { get { return Singleton<OBusSerialization>.Instance; } }

        protected static LionSerializer defaultSerializer = 
             LionSerializers.Json;
        //LionSerializers.FastJson;

        #region Byte Array

        //public static object Deserialize(byte[] bytes, Type type = null, string path = null)
        //{
        //    return SerializationFacility.Deserialize(bytes, type, path);
        //}

        #endregion

        #region Stream

        public static object Deserialize(Stream stream, Type type = null, string path = null)
        {
            return SerializationFacility.Deserialize(stream, type, path);
        }

        public static object Deserialize<T>(Stream stream, string path = null)
        {
            return SerializationFacility.Deserialize<T>(stream);
        }

        public static LionSerializer GetSerializer(object obj = null, string path = null)
        {
            return defaultSerializer;
        }

        //internal static void Serialize(Stream fs, object obj)
        //{            
        //    defaultSerializer.Serialize(fs, obj);
        //}

        #endregion

    }
}
