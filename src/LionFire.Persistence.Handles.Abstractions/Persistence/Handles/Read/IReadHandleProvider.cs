#nullable enable
using LionFire.Referencing;
using System;

namespace LionFire.Persistence.Handles;

public interface IReadHandleProvider
{
    /// <summary>
    /// Get a read handle.  It may be a shared handle.  To ensure a new handle, use IReadHandleCreator.CreateReadHandle instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reference"></param>
    /// <returns></returns>
    /// <remarks>
    /// This asks the corresponding IReadHandleProvider<TReference> to try to upcast the IReference to TReference
    /// 
    /// Example implementation:
    ///   IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => (reference is FileReference fileReference) ? GetReadHandle<T>(fileReference) : null;
    /// </remarks>
    IReadHandle<T>? GetReadHandle<T>(IReference reference); // RENAME Try...
}


//[AutoRegister]
public interface IReadHandleProvider<TReference> 
    //: IReadHandleProvider  // REVIEW - do I need this?  It burdens all the implementers to cast from IReference to TReference.
    where TReference : IReference
{
    IReadHandle<T> GetReadHandle<T>(TReference reference);

}

public static class IReadHandleProviderExtensions
{
    public static IReadHandle<T> GetReadHandle<TReference,T>(this IReadHandleProvider<TReference> hp, IReference<T> reference)
        where TReference : IReference
    {
        return hp.GetReadHandle<T>((TReference)reference);
    }
}


public interface IPreresolvableReadHandleProvider : IReadHandleProvider
{
    IReadHandle<T>? GetReadHandlePreresolved<T>(IReference reference, T preresolvedValue = default); // RENAME Try...
}

public interface IPreresolvableReadHandleProvider<TReference> : IReadHandleProvider<TReference>, IPreresolvableReadHandleProvider
    where TReference : IReference
{
    IReadHandle<T> GetReadHandlePreresolved<T>(TReference reference, T preresolvedValue);
}

public static class ReadHandleProviderX
{
    public static IReadHandle<T>? GetReadHandle<T, TReference>(this IReadHandleProvider<TReference> provider, IReference reference)
        where TReference : IReference 
        => provider.GetReadHandle<T>((TReference)reference);
}


#region HandleCreator TODO

public interface IReadHandleCreator
{
    IReadHandle<T>? CreateReadHandle<T>(IReference reference, T? preresolvedValue = default); // RENAME Try...
}
public interface IReadHandleCreator<TReference>
    where TReference : IReference
{
    IReadHandle<T> CreateReadHandle<T>(TReference reference, T? preresolvedValue = default);
}

#endregion