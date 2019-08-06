using LionFire.Referencing;
using System;

namespace LionFire.Persistence
{
    /// <summary>
    /// Returned for Retrieve or ResolveReference operations (which may do a Retrieve).
    /// </summary>
    public interface IReadResult : IPersistenceResult
    {

        //Type[] Types { get; }

    }

}