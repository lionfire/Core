using LionFire.Assets;
using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    
    //// REVIEW
    //public class AssetInstantiator : IInstantiationComponent
    //{
    //    public Type Type { get; set; }
    //    public string Path { get; set; }

    //    public object Affect(object obj, InstantiationContext context = null)
    //    {
    //        obj = Path.Load(Type.FullName);
    //        return obj;
    //    }
    //}

/*public class AssetReferenceInstantiator<T> : IInstantiationComponent, IFactory
    {
        public AssetReference<T> AssetReference{get;set;}
        

        public object Affect(object obj, InstantiationContext context = null)
        {
            var obj = Path.Load(Type);
            return obj;
        }
    }*/
}
