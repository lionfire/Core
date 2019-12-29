using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IInstantiationFactory
    {
        object Create(InstantiationContext context = null);
    }
    //public interface IFactory<T>
    //{
    //    T Create(InstantiationContext context = null);
    //}
}
