using System;
using System.Collections.Generic;

namespace LionFire.Types
{
    //public class TypeNameRegistryInitializer
    //{
    //    public TypeNameRegistryInitializer(IEnumerable<KeyValuePair<string, Type>> typeNames)
    //    {
    //        TypeNames = typeNames;
    //    }

    //    public IEnumerable<KeyValuePair<string, Type>> TypeNames { get; }

    //    public void Initialize(TypeNameRegistry typeNameRegistry)
    //    {
    //        foreach (var kvp in TypeNames) typeNameRegistry.Types.Add(kvp.Key, kvp.Value);
    //    }
    //}

    public class TypeNameRegistry
    {
        public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
        public Dictionary<Type, string> TypeNames { get; } = new Dictionary<Type, string>();

        //public TypeNameRegistry(IEnumerable<TypeNameRegistryInitializer> initializers)
        //{
        //    foreach (var initializer in initializers) initializer.Initialize(this);
        //}
    }
}
