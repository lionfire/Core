﻿using LionFire.Data;
using LionFire.Data.Async.Sets;
using LionFire.Structures;

namespace LionFire.Data.Async.Collections;

/// <summary>
/// Adds a new instance of a specified type.  
/// 
/// If this object is a collection, the newly created instance should typically be added to the collection
/// 
/// Implementers may support: 
///  - non-concrete types
///  - DI
///  - Identity generation
/// </summary>
/// <remarks>
/// Suggested companion interface: ISupportsTypes and/or ISupportsTypesAsync
/// </remarks>
/// <typeparam name="TValue"></typeparam>
public interface IObservableCreatesAsync<TValue>: ICreatesAsync<TValue>
{
    IObservable<(Type, object[]?, Task<TValue>)> Creates { get; }
}

