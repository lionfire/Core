using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{

    public class MultiType : IMultiTyped, IContainsMultiTyped
    {
        protected Dictionary<Type, object> TypeDict { get { return typeDict; } }
        protected Dictionary<Type, object> typeDict;

        public IEnumerable<Type> Types { get { if (TypeDict == null) return Enumerable.Empty<Type>(); else return TypeDict.Keys; } }

        public MultiType MultiTyped
        {
            get
            {
                return this;
            }
        }

        public T AsType<T>()
            where T : class
        {
            if (typeDict == null) return null;
            if (!typeDict.ContainsKey(typeof(T))) return null;
            return (T) typeDict[typeof(T)] ;
        }

        public void SetType<T>(T obj)
            where T : class
        {
            if (obj == default(T)) { UnsetType<T>(); return; }

            if (typeDict == null)
            {
                typeDict = new Dictionary<Type, object>();
            }
            if (typeDict.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"Already contains an object of type {typeof(T).Name}.  Either remove the previous value or use the Add method to add to a IEnumerable<T> for the type.");
            }
            typeDict.Add(typeof(T), obj);
        }

        public bool UnsetType<T>()
        {
            if (typeDict == null) return false;
            bool foundItem = false;
            if (typeDict.ContainsKey(typeof(T)))
            {
                typeDict.Remove(typeof(T));
                foundItem = true;
            }
            if (typeDict.Count == 0) typeDict = null;

            return foundItem;
        }
    }
    
}
