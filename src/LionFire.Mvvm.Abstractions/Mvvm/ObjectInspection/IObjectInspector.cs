using ReactiveUI;
using System.Security.Cryptography;

namespace LionFire.Mvvm.ObjectInspection;

public interface IObjectInspector
{
    /// <summary>
    /// If the object would benefit from being inspected as another object type, create and return that type.
    /// If that object would also in turn benefit from being inspected as another object type, create that type as well
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    IEnumerable<object> GetInspectedObjects(object @object);
}
