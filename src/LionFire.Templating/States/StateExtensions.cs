using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.States
{
    public static class StateExtensions
    {
        
        public static Dictionary<string, object> GetState(this object obj)
        {
            var m = mis.GetOrAdd(obj.GetType(), t => GetMemberInfos(t));

            var x = new Dictionary<string, object>();

            foreach (var mi in m.PropertyInfos)
            {
                x.Add(mi.Name, mi.GetValue(obj));
            }
            foreach (var mi in m.FieldInfos)
            {
                x.Add(mi.Name, mi.GetValue(obj));
            }

            return x;
        }

        public static void SetState(this object obj, Dictionary<string, object> state)
        {
            var m = mis.GetOrAdd(obj.GetType(), t => GetMemberInfos(t));

            foreach (var kvp in state)
            {
                {
                    var mi = obj.GetType().GetProperty(kvp.Key);
                    if (mi != null)
                    {
                        mi.SetValue(obj, kvp.Value);
                        continue;
                    }
                }
                {
                    var mi = obj.GetType().GetField(kvp.Key);
                    if (mi != null)
                    {
                        mi.SetValue(obj, kvp.Value);
                        continue;
                    }
                }
                Debug.WriteLine("Warning: Property or field not found during SetState: " + kvp.Key); 
            }
        }

        #region Private

        private static MemberInfos GetMemberInfos(Type type)
        {
            var pis = new List<PropertyInfo>();
            var fis = new List<FieldInfo>();
            foreach (var mi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(_ => _.GetCustomAttribute(typeof(StateAttribute)) != null))
            {
                pis.Add(mi);
            }
            foreach (var mi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(_ => _.GetCustomAttribute(typeof(StateAttribute)) != null))
            {
                fis.Add(mi);
            }
            return new MemberInfos { PropertyInfos = pis, FieldInfos = fis };
        }
        private static ConcurrentDictionary<Type, MemberInfos> mis = new ConcurrentDictionary<Type, MemberInfos>();

        private class MemberInfos
        {
            public List<PropertyInfo> PropertyInfos;
            public List<FieldInfo> FieldInfos;
        }

        #endregion

    }
}
