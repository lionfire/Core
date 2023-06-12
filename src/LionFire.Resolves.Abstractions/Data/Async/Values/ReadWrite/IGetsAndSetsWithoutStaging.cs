using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data.Async;

public interface IGetsAndSetsWithoutStaging<TValue>
{
    IGets<TValue> Resolves { get; }
    ISets Commits { get; }
}
