using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Resolvables
{
    public interface IResolver<in TInput, out TOutput>
    {
        //TOutput Resolve<T>(T resolvable) where T : TInput;
        TOutput Resolve(TInput resolvable);
    }
}
