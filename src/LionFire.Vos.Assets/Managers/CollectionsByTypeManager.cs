using LionFire.Referencing;
using LionFire.Vos;
using System;

namespace LionFire.Vos.Collections.ByType
{
    public class CollectionsByTypeManager : ICollectionsByTypeManager, ICollectionTypeProvider
    {
        public IVob Vob { get; }

        public CollectionsByTypeManager(IVob vob)
        {
            Vob = vob;
        }

        public Type GetType(Vob vob)
        {
            if (vob.Parent != Vob) return null;
            return ToType(vob.Name);
        }
        public Type ToType(string typeName)
        {
            throw new NotImplementedException();
        }
        public string ToTypeName(Type type) => throw new NotImplementedException();

        public IReference GetDirectoryForType(Type type) => Vob.Reference.GetChild(ToTypeName(type));
        public Type GetCollectionType(IVob vob) => throw new NotImplementedException();
    }

}
