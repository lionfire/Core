using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;
using LionFire.Types;
using LionFire.ExtensionMethods;

namespace LionFire
{
  

    public static class TypeResolver 
    {
        #region Static

        // FUTURE: Use Injection.GetService
        public static ITypeNamingContext Default { get { return ManualSingleton<ITypeNamingContext>.GuaranteedInstance; } set { ManualSingleton<ITypeNamingContext>.Instance = value; } }
        
        public static Type TryResolve(string typeName, ITypeNamingContext context = null) => (context ?? Default).TryResolve(typeName);
        public static Type Resolve(string typeName, ITypeNamingContext context = null) => (context ?? Default).Resolve(typeName);

        #endregion

        public static Type ToType(this string typeName) => TryResolve(typeName);

    }
}

