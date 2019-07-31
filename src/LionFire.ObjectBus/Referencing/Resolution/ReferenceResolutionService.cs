using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Referencing.Persistence
{
    public class ReferenceResolutionService : IReferenceResolutionService
    {
        public IEnumerable<IReferenceResolutionStrategy> Strategies { get; }

        //PreferredStrategies
        //AllowedStrategies
        //AutomigrateStrategies
        //MigrateStrategies
        //NotFoundStrategies

        public ReferenceResolutionService(IEnumerable<IReferenceResolutionStrategy> strategies)
        {
            this.Strategies = strategies;
        }

        public Task<IEnumerable<ReadResolutionResult<T>>> ResolveAll<T>(IReference reference, ResolveOptions options = null) => throw new NotImplementedException();
        public Task<IEnumerable<WriteResolutionResult<T>>> ResolveAllForWrite<T>(IReference reference, ResolveOptions options = null) => throw new NotImplementedException();
    }

}
