using LionFire.Referencing;
using LionFire.Types;
using LionFire.Vos;
using System;
using LionFire.ExtensionMethods;

namespace LionFire.Vos.Collections.ByType
{
    public class CollectionsByTypeManager : ICollectionsByTypeManager, ICollectionTypeProvider
    {
        public IVob Vob { get; }
        TypeNameRegistry TypeNameRegistry { get; }

        public CollectionsByTypeManager(IVob vob, TypeNameRegistry typeNameRegistry)
        {
            Vob = vob;
            TypeNameRegistry = typeNameRegistry;
        }

        public Type GetType(Vob vob)
        {
            if (vob.Parent != Vob) return null;
            return ToType(vob.Name);
        }
        public Type ToType(string typeName)
        {
            return TypeNameRegistry.Types.TryGetValue(typeName);
        }

        public string ToTypeName(Type type) => throw new NotImplementedException();

        public IReference GetDirectoryForType(Type type) => Vob.Reference.GetChild(ToTypeName(type));
        public Type GetCollectionType(IVob vob) => ToType(vob.Name);
    }

}
