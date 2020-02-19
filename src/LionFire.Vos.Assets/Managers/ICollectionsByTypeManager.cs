using System;

namespace LionFire.Vos.Collections.ByType
{
    public interface ICollectionsByTypeManager
    {
        Type ToType(string typeName);
        string ToTypeName(Type type);
    }

}
