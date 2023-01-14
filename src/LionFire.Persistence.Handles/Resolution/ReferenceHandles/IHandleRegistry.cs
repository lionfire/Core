#nullable enable
using System;
using LionFire.Persistence;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Primary Keys for handles:
///  - Handle kind: Read-only, Read-write, Write-only
///  - Value Type
///  - URI (including schema)
/// </summary>
public interface IHandleRegistry
{
    IReadWriteHandle<TValue> GetOrAddReadWrite<TValue>(string uri, Func<string, IReadWriteHandle<TValue>> factory);
    IWriteHandle<TValue> GetOrAddWrite<TValue>(string uri, Func<string, IWriteHandle<TValue>> factory);
    IReadHandle<TValue> GetOrAddRead<TValue>(string uri, Func<string, IReadHandle<TValue>> factory);

    IReadWriteHandle<TValue>? TryGetReadWrite<TValue>(string uri);
    IWriteHandle<TValue>? TryGetWrite<TValue>(string uri);
    IReadHandle<TValue>? TryGetRead<TValue>(string uri);

}
