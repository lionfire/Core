using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Types
{
    [DefaultConcreteType(typeof(TypeNamingContext))]
    public interface ITypeNamingContext
    {
        Type Resolve(string typeName);
        Type TryResolve(string typeName);

        void Register(string typeName, Type type);  
    }

    
}
