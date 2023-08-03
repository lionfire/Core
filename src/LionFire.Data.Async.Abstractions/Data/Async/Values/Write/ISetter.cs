﻿#nullable enable
using LionFire.Results;
using LionFire.Structures;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Sets;

public interface ISetter<in T>
{
    /// <summary>
    /// Set the current Value to value, and initiate a Put to the underlying data store with that value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<ITransferResult> Set(T? value, CancellationToken cancellationToken = default);
}