#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology; 

/// <summary>
/// Object "is"/has a particular type, and the type will be returned upon a call to ObjectAsType<typeparamref name="T"/>()
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHas<out T>
{
    T? Object { get; }
}

// RENAME IHas to IHasNullable, and IHasHonNull to IHas

public interface IHasNonNull<out T>
{
    T Object { get; }
}

/// <typeparam name="T"></typeparam>
/// <seealso cref="IDependsOn"/>
public interface IHasNonNullSettable<T> : IHasNonNull<T>
{
    new T Object { get; set; }
}