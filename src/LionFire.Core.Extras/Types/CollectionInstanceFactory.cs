using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace LionFire.Types
{

    // REVIEW: Doesn't look like this class is finished yet.

    public class CollectionInstanceFactory
    {
        public static CollectionInstanceFactory Default
        {
            get { return Singleton<CollectionInstanceFactory>.Instance; }
        }

        #region Collections

        public ConcurrentDictionary<Type, Type> ConcreteCollectionTypes = new ConcurrentDictionary<Type, Type>(); // MOVE - Non-generic class Configuration 

        #endregion

        public Type TryGetConcreteType(Type interfaceType)
        {
            Type concreteType;

            if (ConcreteCollectionTypes.TryGetValue(interfaceType, out concreteType))
            {
                return concreteType;
            }
            return null;
        }

        public T TryCreate<T>(Type interfaceType)
            where T : class
        {
            Type concreteType = TryGetConcreteType(interfaceType);
            if (concreteType == null) return (T)null;

            T result = (T)Activator.CreateInstance(concreteType);
            return result;
        }

        public T Create<T>(Type interfaceType)
            where T : class
        {
            return (T)Create(interfaceType);
        }
        public object Create(Type interfaceType)
        {
            if (!interfaceType.IsGenericType) throw new ArgumentException("interfaceType must be a generic type");

            Type interfaceTypeGeneric = interfaceType.GetGenericTypeDefinition();

            Type concreteTypeGeneric = TryGetConcreteType(interfaceTypeGeneric);
            if (concreteTypeGeneric == null) { throw new ArgumentException("No concrete type registered for specified interfaceType: " + interfaceType); }

            Type concreteType = concreteTypeGeneric.MakeGenericType(interfaceType.GetGenericArguments());


            object result = Activator.CreateInstance(concreteType);
            return result;
        }

        //		public object Create(Type interfaceType, Type elementType)
        //		{
        // AOT Replacement?
        //		}
    }

    //public class InstanceFactory
    //{

    //}
}
