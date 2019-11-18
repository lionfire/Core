using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Dependencies;

namespace LionFire.Referencing
{
    public interface IReferenceProvider : ISupportsUriSchemes
    {
        IReference TryGetReference(string uri);

        IEnumerable<Type> ReferenceTypes { get; }

    }

    public static class IReferenceProviderExtensions
    {
        public static IReference GetReference(this IReferenceProvider referenceProvider, string uri) => 
            referenceProvider.TryGetReference(uri) ?? throw new NotFoundException();

        
    }

    public interface IReferenceProviderService : IReferenceProvider
    {
    }

    
}
