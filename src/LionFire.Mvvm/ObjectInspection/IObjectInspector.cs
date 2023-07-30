using ReactiveUI;
using System.Security.Cryptography;

namespace LionFire.Mvvm.ObjectInspection;

public interface IObjectInspector
{
    IEnumerable<InspectedObjectItem> GetInspectedObjects(object obj);
}
