using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Dependencies;

namespace LionFire.Referencing;

public interface IReferenceProvider : ISupportsUriSchemes
{
    (TReference result, string error) TryGetReference<TReference>(string uri) where TReference : IReference;

    IEnumerable<Type> ReferenceTypes { get; }

}

public static class IReferenceProviderExtensions
{
    public static TReference GetReference<TReference>(this IReferenceProvider referenceProvider, string uri)
        where TReference : IReference
        => referenceProvider.TryGetReference<TReference>(uri).result ?? throw new NotFoundException();
    
}

public interface IReferenceProviderService : IReferenceProvider
{
}


