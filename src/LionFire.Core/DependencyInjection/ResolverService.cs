using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.DependencyInjection
{
    /// <remarks>
    /// TODO: Figure out how to rank/score the candidates.  
    ///  - ConditionalWeakTable?
    ///  - register wrapped types: new ScoredService<ServiceType>(50) or something
    /// </remarks>
    /// <typeparam name="TResolvedService"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class ResolverService<TResolvedService, TInput> : IResolverService<TResolvedService, TInput>
            where TResolvedService : ICompatibleWithSome<TInput>
    {
        public TResolvedService Resolve(TInput input) => InjectionContext.Current.GetService<IEnumerable<TResolvedService>>().Where(s => s.IsCompatibleWith(input)).FirstOrDefault();
        public IEnumerable<TResolvedService> ResolveAll(TInput input, bool strict = false) => InjectionContext.Current.GetService<IEnumerable<TResolvedService>>().Where(s => s.IsCompatibleWith(input));
    }
}
