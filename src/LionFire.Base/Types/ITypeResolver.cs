#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Types
{
    //[DefaultImplementationType(typeof(TypeResolver))]
    public interface ITypeResolver //: IFreezable
    {
        Type Resolve(string typeName);
        Type? TryResolve(string typeName);

        //void Register(Type type, string? typeName = null);

        //void Register<T>(string? typeName = null);
    }

}
