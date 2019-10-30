using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Resolves
{
    public interface IResolveCommitPair<TValue>
    {
        IResolves<TValue> Resolves { get; }
        IPuts Commits { get; }
    }
}
