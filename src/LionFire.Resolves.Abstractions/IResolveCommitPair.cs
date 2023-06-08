using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data.Async.Gets
{
    public interface IResolveCommitPair<TValue>
    {
        IGets<TValue> Resolves { get; }
        ISets Commits { get; }
    }
}
