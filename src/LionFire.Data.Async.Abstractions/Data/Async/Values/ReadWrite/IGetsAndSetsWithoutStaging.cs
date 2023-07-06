using LionFire.Data.Gets;
using LionFire.Data.Sets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data;

public interface IGetsAndSetsWithoutStaging<TValue>
{
    IGets<TValue> Resolves { get; }
    ISets Commits { get; }
}
