using LionFire.Ontology;
using System;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets.ChainResolving;

public class ChainGetterptions : IParentable<ChainGetterptions>
{
    public ChainGetterptions Parent { get; set; } = ChainGetterptions.Default;

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
    public List<ChainGetterWorker> Resolvers { get; set; } = new List<ChainGetterWorker>();

    public IEnumerable<ChainGetterWorker> AllResolvers
    {
        get
        {
            if(Resolvers != null) foreach (var r in Resolvers) yield return r;
            if (Parent != null && ReferenceEquals(Parent, this)) foreach (var r in Parent.Resolvers) yield return r;
        }
    }

    public static ChainGetterptions Default = new ChainGetterptions
    {
        Resolvers = new List<ChainGetterWorker>
        {
            new ChainGetterWorker(typeof(IGetter<object>), o => ((IGetter<object>)o).GetIfNeeded()), // Put this first, because it caches results
            new ChainGetterWorker(typeof(IStatelessGetter<object>), o => ((IStatelessGetter<object>)o).Get()), // Re-evaluates every time -- may be a long I/O operation
        }
    };
}
