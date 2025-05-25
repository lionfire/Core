using LionFire.Referencing;
using System;

namespace LionFire.Persistence.Handles;

public interface IHandleBase : IReferenceable, IPersists
{
    Type Type { get; }
}

//public interface IHandle<T> : IPersisted<T> { }
