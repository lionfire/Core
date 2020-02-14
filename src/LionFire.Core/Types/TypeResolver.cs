#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Types;
using LionFire.ExtensionMethods;
using LionFire.Structures;
using Microsoft.Extensions.Options;

namespace LionFire.Types
{
    // TODO: Cleanup
    // TODO: Option for falling back to .NET Type.GetType()

    public class TypeResolver : ITypeResolver
    {
        internal IDictionary<string, Type?> Types => TypeNameRegistry.Types;

        TypeNameRegistry TypeNameRegistry { get; }
        public TypeResolver(TypeNameRegistry typeNameRegistry)
        {
            TypeNameRegistry = typeNameRegistry;
        }


        //#region Register

        //public void Register(Type type, string? typeName = null)
        //{
        //    if (IsFrozen) throw new ObjectFrozenException();

        //    if (typeName == null) typeName = type.Name;

        //    if (types.ContainsKey(typeName))
        //    {
        //        throw new AlreadyException($"{typeName} is already registered.  Was the same Assembly registered twice, or is there a conflict?");
        //    }
        //    types.GetOrAdd(typeName, x => type);
        //}
        //public void Register<T>(string? typeName = null)
        //{
        //    if (IsFrozen) throw new ObjectFrozenException();
        //    if (typeName == null) typeName = typeof(T).Name;

        //    if (types.ContainsKey(typeName)) throw new AlreadyException();
        //    types.GetOrAdd(typeName, x => typeof(T));
        //}

        //#endregion

        #region Resolve

        public Type? TryResolve(string typeName)
        {
            var type = Types.TryGetValue(typeName);

            if (type != null) return type;

            try
            {
                type = Type.GetType(typeName);
            }
            catch { } // EMPTYCATCH

            return type;
        }

        public Type Resolve(string typeName)
        {
            Type? type = Types.TryGetValue(typeName);
            var result = TryResolve(typeName);
            if (result == null)
            {
                throw new TypeNotFoundException(typeName);
            }
            return result;
        }

        #endregion


        //public void UseShortNamesForAssembly(System.Reflection.Assembly assembly)
        //{
        //    foreach (var type in assembly.GetTypes())
        //    {
        //        if (conflictingShortNames.Contains(type.Name)) { continue; }
        //        else if (types.ContainsKey(type.Name))
        //        {
        //            types.Remove(type.Name); conflictingShortNames.Add(type.Name);
        //            System.Diagnostics.Debug.WriteLine("Type already exists: " + type);
        //            continue;
        //        }

        //        types.Add(type.Name, type);
        //    }
        //}
    }

}
