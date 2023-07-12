using LionFire.Referencing;
using System;
using LionFire.Referencing.Ex;
using LionFire.Dependencies;
using LionFire.Assets;
using LionFire.Data.Id;

namespace LionFire.Persistence;

public static class ObjectToHandleViaIdExtensions
{

    public static IReadHandle<TValue> GetReadHandle<TValue>(this TValue obj)
        where TValue : IIded<string>
        => new IdReference<TValue>(obj.Id).GetReadHandlePreresolved(preresolvedValue: obj).handle;

    public static IWriteHandle<TValue> GetWriteHandle<TValue>(this TValue obj)
        where TValue : IIded<string>
        => new IdReference<TValue>(obj.Id).GetWriteHandle(prestagedValue: obj);
    
    public static IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(this TValue obj)
        where TValue : IIded<string>
        => new IdReference<TValue>(obj.Id).GetReadWriteHandlePrestaged<TValue>(preStagedValue: obj).handle;
}
