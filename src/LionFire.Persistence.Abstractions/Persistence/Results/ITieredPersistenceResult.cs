using System.Collections.Generic;

namespace LionFire.Persistence
{
    public interface ITieredPersistenceResult : ITransferResult
    {

        int RelevantUnderlyingCount { get; set; }


        IEnumerable<ITransferResult> Successes { get; set; }
        IEnumerable<ITransferResult> Failures { get; set; }
    }

}