using LionFire.Referencing.Resolution;
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing.Handles
{
    public class ThrowingReferenceResolver : IHandleResolver
    {
        public Task<ResolveHandleResult<T>> Resolve<T>(IReadHandle<T> handle)
            where T : class
        {
            throw new Exception("No IReferenceResolver available");
        }
    }
}
