#nullable enable
using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LionFire.FlexObjects.Implementation
{
    public class FlexTypeDictionary
    {
        public ConcurrentDictionary<Type, object>? Types => types;
        private ConcurrentDictionary<Type, object>? types;

        public FlexTypeDictionary(params object[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        (Type, object) ResolveType(object item) => item is ITypedObject to ? (to.Type, to.Object) : (item.GetType(), item);

        public bool ContainsKey(object item)
        {
            var (type, _) = ResolveType(item);
            return types?.ContainsKey(type) == true;
        }

        public void Add(Type type, object item)
        {
            if (item == null) throw new ArgumentNullException();

            types ??= new ConcurrentDictionary<Type, object>();
            types.AddOrThrow(type, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">If it is wrapped in ITypedObject, it will be unwrapped appropriately</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(object item)
        {
            if (item == null) throw new ArgumentNullException();

            types ??= new ConcurrentDictionary<Type, object>();

            var (type, actualObject) = ResolveType(item);
            types.AddOrThrow(type, actualObject);
        }

        // THREADUNSAFE
        public bool Remove(object item)
        {
            if (types == null) return false;

            var (type, actualObject) = ResolveType(item);
            if (!types.ContainsKey(type)) { return false; }

            if (types[type] != item) return false;

            // REVIEW - potential data loss in thread race conditions, or failure on bad equality comparison
            if (!types.TryRemove(type, out object? removed)) return false;
            else
            {
                if (types?.Count == 0) types = null;
                if (removed == actualObject) return true;
                else
                {
                    Add(removed);
                    return false;
                }
            }
        }
    }
}
