#if CQRS

using LionFire.Cqrs;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Referencing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Vos.Persisters.Cqrs
{
    public class VosListHandler : IQueryHandler<VosListQuery, VosRetrieveResult<Listing<object>>>
    {
        public Task<VosRetrieveResult<Listing<object>>> Handle(VosListQuery query, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
    public struct VosListQuery
    {
        public string Path { get; set; }
    }
}
#endif