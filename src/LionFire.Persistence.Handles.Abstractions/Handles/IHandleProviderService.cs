using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    public interface IHandleProviderService : IReadWriteHandleProvider
    {
        /// <summary>
        /// Informational
        /// </summary>
        IEnumerable<Type> HandleTypes { get; }
    }
}
