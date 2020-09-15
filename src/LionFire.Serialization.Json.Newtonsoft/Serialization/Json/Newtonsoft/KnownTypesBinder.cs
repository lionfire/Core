using LionFire.Types;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public class KnownTypesBinder : ISerializationBinder
    {
        public TypeNameRegistry TypeNameRegistry { get; }

        ISerializationBinder Default = new DefaultSerializationBinder();

        public KnownTypesBinder(TypeNameRegistry typeNameRegistry)
        {
            TypeNameRegistry = typeNameRegistry;
        }

        public Type BindToType(string assemblyName, string typeName) 
            => TypeNameRegistry.Types.TryGetValue(typeName, out var result) 
            ? result 
            : Default.BindToType(assemblyName, typeName);

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (TypeNameRegistry.TypeNames.ContainsKey(serializedType))
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }
            else
            {
                Default.BindToName(serializedType, out assemblyName, out typeName);
            }
        }
    }
}
