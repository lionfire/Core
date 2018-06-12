using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IReadonlyMultiTyped
    {
        T AsType<T>() where T : class;

    }

    // REVIEW these:

    public interface IReadonlyMultiTypedEx : SReadonlyMultiTypedEx
    {
        IReadOnlyDictionary<Type, object> Types { get; }
        object this[Type type] { get; }
        object[] SubTypes { get; }
    }
    
    //public interface IMultiTyped : SMultiTyped
    //{
    //    object this[Type type] { get; }
    //    //object TryGet(Type type);
    //    object[] SubTypes { get; }
    //    // FUTURE: Get by name or type/name combo
    //}
}
