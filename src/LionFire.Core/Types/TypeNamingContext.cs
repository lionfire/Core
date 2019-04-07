using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Types;
using LionFire.ExtensionMethods;

namespace LionFire.Types
{
    public class TypeNamingContext : ITypeNamingContext
    {
        internal Dictionary<string, Type> Types { get { return Types; } }
        private Dictionary<string, Type> types = new Dictionary<string, Type>();
        HashSet<string> conflictingShortNames = new HashSet<string>();

        public TypeNamingContext()
        {
        }

        public Type TryResolve(string typeName)
        {
            var type = types.TryGetValue(typeName);

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
            var result = TryResolve(typeName);
            if (result == null)
            {
                throw new TypeNotFoundException(typeName);
            }
            return result;
        }

        public void Register(string typeName, Type type)
        {
            if (types.ContainsKey(typeName))
            {
                throw new AlreadyException($"{typeName} is already registered.  Was the same Assembly registered twice, or is there a conflict?");
            }
            types.GetOrAdd(typeName, x => type);
        }
        public void Register<T>(string typeName)
        {
            if (types.ContainsKey(typeName)) throw new AlreadyException();
            types.GetOrAdd(typeName, x => typeof(T));
        }

        public bool UseShortNamesForDataAssemblies { get; set; } = false;


        public void UseShortNamesForAssembly(System.Reflection.Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (conflictingShortNames.Contains(type.Name)) { continue; }
                else if (types.ContainsKey(type.Name))
                {
                    types.Remove(type.Name); conflictingShortNames.Add(type.Name);
                    System.Diagnostics.Debug.WriteLine("Type already exists: " + type);
                    continue;
                }

                types.Add(type.Name, type);
            }
        }
    }

}
