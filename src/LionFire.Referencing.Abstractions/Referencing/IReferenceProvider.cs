using System;
using System.Collections.Generic;
using System.Text;
using LionFire.DependencyInjection;

namespace LionFire.Referencing
{ 
    public interface IReferenceProvider : ISupportsUriSchemes, ICompatibleWithSome<string>
    {
        IEnumerable<Type> ReferenceTypes { get; }
        IReference TryGetReference(string uri, bool strictMode = false);
        bool IsValid(IReference reference);
    }

}
