using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Collections
{
    public class CollectionTypeName
    {
        public string TypeName { get; set; }
    }
    public class CollectionType
    {
        
        public Type Type { get; set; }

    }

    public static class CollectionTypeExtensions
    {
        public static Type GetVobCollectionType(this IVob vob)
        {
            var type = vob.TryGetOwnVobNode<CollectionType>()?.Value?.Type;
            if(type != null) { return type; }

            //type = vob.Reference.GetCollectionType();

            //var ctm = vob.AcquireNext<CollectionsByTypeManager>(1, 1);
            //if(ctm != null) { type = ctm.}

            return type;
        }
        public static void SetCollectionType<T>(this IVob vob) => vob.AddOwn(new CollectionType { Type = typeof(T) });
    }
}
