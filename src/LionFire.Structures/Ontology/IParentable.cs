#nullable enable

namespace LionFire.Ontology;

public interface IParented
{
    object Parent { get; set; }
}
public interface IParentable<T> : IParented<T>
{
    new T? Parent { get;  set; }
}

public interface IParented<out T>
{
    T? Parent { get; }
}
