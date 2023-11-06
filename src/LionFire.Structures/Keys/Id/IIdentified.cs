#nullable enable

namespace LionFire.Structures;

public interface IIdentified<T>
{
    T? Id { get; }
}

public interface IIdentifiable<T> : IIdentified<T>
{
    new T? Id { get; set; }
}