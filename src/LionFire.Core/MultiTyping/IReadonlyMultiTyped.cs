using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    
    public interface IReadOnlyMultiTyped : SReadOnlyMultiTyped // REVIEW MOVE - should this be in LionFire.Structures?
    {
        //T AsType<T>() where T : class;

        object? this[Type type] { get; }

        IEnumerable<object> SubTypes { get; } // TODO - change to IEnumerable<object>

    }

    // REVIEW this -- add these back in and remove Ex interface?

    public interface IReadOnlyMultiTypedEx : IReadOnlyMultiTyped
    {
        IReadOnlyDictionary<Type, object> Types { get; }
        
    }
    
    //public interface IMultiTyped : SMultiTyped
    //{
    //    object this[Type type] { get; }
    //    //object TryGet(Type type);
    //    object[] SubTypes { get; }
    //    // FUTURE: Get by name or type/name combo
    //}
}
