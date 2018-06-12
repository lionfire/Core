using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LionFire.Serialization
{
    public static class SerializationReflection
    {
        public class TypeSerializationInfo
        {
            public List<FieldInfo> FieldInfos = new List<FieldInfo>();
            public List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();
            public bool ByValue = false;
        }

        private static Dictionary<Type, TypeSerializationInfo> typeSerializationInfos = new Dictionary<Type, TypeSerializationInfo>();
        private static object typeSerializationInfosLock = new object();

        public const BindingFlags MemberBindingFlags =  BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default ;
        public static readonly bool SerializeReadOnly = true;

        public static TypeSerializationInfo GetSerializationInfo(Type type)
        {            
            if (typeSerializationInfos.ContainsKey(type))
            {
                return typeSerializationInfos[type];
            }
            lock (typeSerializationInfosLock)
            {
                if (typeSerializationInfos.ContainsKey(type))
                {
                    return typeSerializationInfos[type];
                }
                var tsi = new TypeSerializationInfo();

                var attr = TypeExtensions.GetCustomAttribute<LionSerializableAttribute>(type, true); // REFLECTION OPTIMIZE
                tsi.ByValue = attr != null && attr.Method == SerializeMethod.ByValue;
                

                foreach (FieldInfo mi in type.GetFields(MemberBindingFlags))
                {
                    if (!SerializeReadOnly && mi.IsInitOnly) continue;
                    if (mi.IsNotSerialized) continue;

                    tsi.FieldInfos.Add(mi);
                }
                tsi.FieldInfos.Sort(new Comparison<FieldInfo>((l, r) => l.Name.CompareTo(r.Name)));

                foreach (PropertyInfo mi in type.GetProperties(MemberBindingFlags))
                {
                    if (!mi.CanRead || !mi.CanWrite) continue;
                    if (mi.GetIndexParameters().Length > 0) continue;

                    tsi.PropertyInfos.Add(mi);
                }
                tsi.PropertyInfos.Sort(new Comparison<PropertyInfo>((l, r) => l.Name.CompareTo(r.Name)));

                typeSerializationInfos.Add(type, tsi);

                return tsi;
            }
        }

    }
}
