using System;
using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif
using System.Linq;
using System.Text;

namespace LionFire.Serialization
{
    public static class GaxaSerializer
    {
        public static string Serialize(object obj)
        {
#if UNITY
return JsonUtility.ToJson(obj);
#else
            
#endif
        }

        public static object Deseraialize(string json, Type type)
        {
#if UNITY
            return JsonUtility.FromJson(json, type);
#else
            
#endif

        }
    }
}
