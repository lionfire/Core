using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive;

public static class OnDemandX
{
    public static IObservable<IChangeSet<TObject, TKey>> ConnectOnDemand<TObject, TKey>(
  this SourceCache<TObject, TKey> sourceCache,
  Func<SourceCache<TObject, TKey>, IDisposable> resourceFactory,
  Func<TObject, bool>? predicate = null,
  bool suppressEmptyChangeSets = true)
  where TKey : notnull
  where TObject : notnull
    {
        return Observable.Using(() => resourceFactory(sourceCache),
                    _ => sourceCache.Connect(predicate, suppressEmptyChangeSets)
                ).RefCount();
    }

    public static IObservable<IChangeSet<TObject, TKey>> ConnectOnDemand<TObject, TKey>(
  this SourceCache<TObject, TKey> sourceCache,
  Func<IDisposable> resourceFactory,
  Func<TObject, bool>? predicate = null,
  bool suppressEmptyChangeSets = true)
  where TKey : notnull
  where TObject : notnull
    {
        return Observable.Using(resourceFactory,
                    _ => sourceCache.Connect(predicate, suppressEmptyChangeSets)
                ).RefCount();
    }
}
