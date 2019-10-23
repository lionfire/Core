using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Reflection;

namespace LionFire.Persistence.Handles
{
    public interface IHandleBase : IReferencable, IHasPersistenceState // RENAME to IHandle, maybe
    {
    }

    public class IHandleBaseExtensionsCache<T>
    {
        static IHandleBaseExtensionsCache()
        {
            get_ObjectPropertyInfo = typeof(T).GetType().GetProperty("Object");
        }
        internal static PropertyInfo get_ObjectPropertyInfo { get; set; }
    }
    public static class IHandleBaseExtensions
    {
        public static T Object<T>(this IHandleBase handleBase)
        {
            return (T)IHandleBaseExtensionsCache<T>.get_ObjectPropertyInfo.GetValue(handleBase);
        }
    }
}

namespace LionFire.Persistence
{
    public interface IHandleEx<T> : IReadHandleEx<T>, H<T> { }

    public interface H<T> : RH<T>, IWrapper<T>, ICommitable, IDeletable, IWriteHandle<T>, IHandleBase { } // Rename to W?  And write-only: WO

}
