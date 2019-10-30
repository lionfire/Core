using System.Reflection;

namespace LionFire.Persistence.Handles
{
    // REVIEW TODESIGN

    public class IHandleBaseExtensionsCache<T>
    {
        static IHandleBaseExtensionsCache()
        {
            get_ObjectPropertyInfo = typeof(T).GetType().GetProperty("Object");
        }
        internal static PropertyInfo get_ObjectPropertyInfo { get; set; }
    }
}
