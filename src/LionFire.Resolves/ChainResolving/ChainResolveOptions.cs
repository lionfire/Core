using LionFire.Ontology;
using System;
using System.Collections.Generic;

namespace LionFire.Resolves.ChainResolving
{
    public class ChainResolveOptions : IParented<ChainResolveOptions>
    {
        public ChainResolveOptions Parent { get; set; } = ChainResolveOptions.Default;

        public int? MaxResolveCount { get; set; }

        /// <summary>
        /// If false, stop resolvin
        /// </summary>
        public Predicate<object> ContinueResolveCondition { get; set; }

        /// <summary>
        /// If true, replace the stored value with the resolved value
        /// If missing, defaults to true
        /// </summary>
        public Func<object, object, bool> ReplaceValueCondition { get; set; }

        /// <summary>
        /// List of things to try
        /// </summary>
        public List<ChainResolverWorker> Resolvers { get; set; } = new List<ChainResolverWorker>();

        public IEnumerable<ChainResolverWorker> AllResolvers
        {
            get
            {
                if(Resolvers != null) foreach (var r in Resolvers) yield return r;
                if (Parent != null && ReferenceEquals(Parent, this)) foreach (var r in Parent.Resolvers) yield return r;
            }
        }

        public static ChainResolveOptions Default = new ChainResolveOptions
        {
            Resolvers = new List<ChainResolverWorker>
            {
                new ChainResolverWorker(typeof(ILazilyResolves<object>), o => ((ILazilyResolves<object>)o).TryGetValue()), // Put this first, because it caches results
                new ChainResolverWorker(typeof(IResolves<object>), o => ((IResolves<object>)o).Resolve()), // Re-evaluates every time -- may be a long I/O operation
            }
        };
    }
}
