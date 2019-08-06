using System.Collections.Generic;

namespace LionFire.Persistence
{
    public interface ITieredPersistenceResult : IPersistenceResult
    {

        int RelevantUnderlyingCount { get; set; }


        IEnumerable<IPersistenceResult> Successes { get; set; }
        IEnumerable<IPersistenceResult> Failures { get; set; }
    }

}