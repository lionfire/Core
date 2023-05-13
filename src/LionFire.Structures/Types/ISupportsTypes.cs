namespace LionFire.Structures;

/// <summary>
/// One typical use is for polymorphic generic types that typically take an Interface as parameter.  This interface provides awareness of which concrete types are available for the interface.  This can be useful when adding new items to a collection.
/// </summary>
public interface ISupportsTypes
{
    IEnumerable<Type> SupportedTypes { get; }
}
