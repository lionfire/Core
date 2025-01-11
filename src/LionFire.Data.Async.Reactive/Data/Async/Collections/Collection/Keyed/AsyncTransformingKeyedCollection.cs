using LionFire.Data.Async;
using LionFire.Data.Async.Sets;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LionFire.Data.Collections;

public abstract class AsyncTransformingKeyedCollection<TKey, TUnderlying, TUsable> : AsyncTransformingReadOnlyKeyedCollection<TKey, TUnderlying, TUsable>
    , IAsyncKeyedCollection<TKey, TUsable>
    where TKey : notnull
    where TUnderlying : notnull
    where TUsable : notnull
{

    #region Lifecycle

    protected AsyncTransformingKeyedCollection(AsyncKeyedCollection<TKey, TUnderlying> underlying) : base(underlying)
    {
    }

    #endregion

    public override bool IsReadOnly => false;

    public virtual ValueTask Add(TUsable item)
    {
        return underlying.Add(Serialize(item));
    }

    #region Mutation

    public virtual ValueTask<bool> Remove(TUsable item) => throw new NotSupportedException();

    public virtual ValueTask<bool> Remove(TKey key) => underlying.Remove(key);

    public ValueTask Upsert(TUsable item)
    {
        throw new NotImplementedException();
    }

    //public override Task<bool> Remove(TUsable item) => throw new NotSupportedException(); // Remove(KeySelector(item));

    #endregion

}

