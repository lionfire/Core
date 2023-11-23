#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Dependencies;

namespace LionFire.Referencing;

public interface IReferenceProvider : ISupportsUriSchemes
{
    (TReference? result, string? error) TryGetReference<TReference>(string uri, bool aggregateErrors = false) 
        where TReference : IReference;

    IEnumerable<Type> ReferenceTypes { get; }

}

public static class IReferenceProviderExtensions
{
    
    
}

public interface IReferenceProviderService : IReferenceProvider
{
}


