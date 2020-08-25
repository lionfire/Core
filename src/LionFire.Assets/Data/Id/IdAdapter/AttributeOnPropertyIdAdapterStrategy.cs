using System.Linq;
using System.Reflection;

namespace LionFire.Data.Id
{
    public class AttributeOnPropertyIdAdapterStrategy : IIdMappingStrategy
    {
        private class ObjectIdProperties<T>
        {
            public static PropertyInfo Property { get; }

            static ObjectIdProperties()
            {
                Property = typeof(T).GetProperties().Where(mi => mi.GetCustomAttribute<IdAttribute>() != null).SingleOrDefault();
            }
        }

        public (bool, string) TryGetId(object obj)
        {
            var pi = (PropertyInfo)typeof(ObjectIdProperties<>).MakeGenericType(obj.GetType()).GetProperty(nameof(ObjectIdProperties<object>.Property)).GetValue(null);

            if (pi == null) { return (false, default); }
            return (true, (string)pi.GetValue(obj));
        }
    }
}
