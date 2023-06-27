using LionFire.Ontology;
using System;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets.ChainResolving
{
    public class ChainResolveOptions : IParentable<ChainResolveOptions>
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
                new ChainResolverWorker(typeof(ILazilyGets<object>), o => ((ILazilyGets<object>)o).GetIfNeeded()), // Put this first, because it caches results
                new ChainResolverWorker(typeof(IGets<object>), o => ((IGets<object>)o).Get()), // Re-evaluates every time -- may be a long I/O operation
            }
        };
    }
}
