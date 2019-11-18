using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Dependencies
{
    public interface IResolverService<TResolvedService, TInput>
        where TResolvedService : ICompatibleWithSome<TInput>
    {
        TResolvedService Resolve(TInput input);
        IEnumerable<TResolvedService> ResolveAll(TInput input, bool strict = false); // FUTURE?
    }
}
