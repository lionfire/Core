using System;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    public interface IHandleProviderService : IHandleProvider
    {
        /// <summary>
        /// Informational
        /// </summary>
        IEnumerable<Type> HandleTypes { get; }
    }
}
