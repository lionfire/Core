using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    public class OptionalRef<Type>
        where Type : class
    {
        public OptionalRef(Type val) { this.Value = val; }
        //private OptionalRef(DBNull _) { this.Value = null; }
        public Type Value { get; private set; }

        public static readonly OptionalRef<Type> Empty = new OptionalRef<Type>(null);

        public static implicit operator OptionalRef<Type>(Type v)
        {
            if (v == null) return null;
            return new OptionalRef<Type>(v);
        }
    }

    public static class OptionalRefExtensions
    {

        /// <summary>
        /// Creates an OptionalRef object only if the specified parameter is non-null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static OptionalRef<Type> ToOptionalRef<Type>(this Type obj)
            where Type : class
        {
            if (obj == null) { return OptionalRef<Type>.Empty; }

            return new OptionalRef<Type>(obj);
        }
    }
}
