using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    public interface IHandleProviderService : IHandleProvider
    {
        /// <summary>
        /// Informational
        /// </summary>
        IEnumerable<Type> HandleTypes { get; }
    }
}
