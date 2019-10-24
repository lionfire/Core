using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IFactory
    {
        object Create(InstantiationContext context = null);
    }
    //public interface IFactory<TValue>
    //{
    //    TValue Create(InstantiationContext context = null);
    //}
}
