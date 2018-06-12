using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DefaultConcreteTypeAttribute : Attribute
    {
        public Type Type { get { return type; } }
        private readonly Type type;

        public DefaultConcreteTypeAttribute(Type type)
        {
            if (type.IsAbstract || type.IsInterface) throw new ArgumentException("type must not be abstract or interface");
            this.type = type;
        }
    }
}
