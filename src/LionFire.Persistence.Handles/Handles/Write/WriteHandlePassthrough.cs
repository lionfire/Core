﻿using LionFire.Data.Async.Sets;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Results;
using LionFire.Structures;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Convenience class that combines Reference and Handle
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TReference"></typeparam>
public class WriteHandlePassthrough<TValue, TReference> : IWriteHandle<TValue>, IReferenceable<TReference>
    where TReference : IReference
{
    public TReference Reference { get; set; }

    public IWriteHandle<TValue> WriteHandle => handle ??= Reference?.GetWriteHandle<TValue>();

    public TValue Value { get => WriteHandle.Value; set => WriteHandle.Value = value; }

    TValue IReadWrapper<TValue>.Value => WriteHandle.Value;

    TValue IWriteWrapper<TValue>.Value { set => WriteHandle.Value = value; }

    public bool HasValue => WriteHandle.HasValue;

    public Type Type => WriteHandle.Type;

    IReference IReferenceable.Reference => WriteHandle.Reference;

    public PersistenceFlags Flags => WriteHandle.Flags;

    public TValue? StagedValue { get => WriteHandle.StagedValue; set => WriteHandle.StagedValue = value; }
    public bool HasStagedValue { get => WriteHandle.HasStagedValue; set => WriteHandle.HasStagedValue = value; }

    protected IWriteHandle<TValue> handle;

    public Task<ISetResult> Set(CancellationToken cancellationToken = default) => WriteHandle.Set(cancellationToken);
    public Task<bool?> Delete() => WriteHandle.Delete();
    public void MarkDeleted() => WriteHandle.MarkDeleted();
    public void DiscardValue() => WriteHandle.DiscardValue();
    public Task<ISetResult<TValue>> Set(TValue value, CancellationToken cancellationToken = default) => WriteHandle.Set(value, cancellationToken);

    public void DiscardStagedValue()
    {
        WriteHandle.DiscardStagedValue();
    }
}
