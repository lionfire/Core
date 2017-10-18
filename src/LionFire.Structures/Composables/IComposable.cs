using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Composables
{

    public interface IComposition
    {
        IEnumerable<object> Children { get; }
    }

    //public interface IComposition<T>
    //{
    //    IEnumerable<T> Children { get; }
    //}

    //public interface IComposable<TComposable, TChildType> : IComposition<TChildType>
    //    where TChildType : class
    //{
    //    TComposable Add(TChildType component);
    //}

    public interface IComposable<TComposable>
    : IComposition
    {
        // REVIEW - does it make sense to impose a fluent interface?
        TComposable Add<T>(T component) where T : class;
    }

}
