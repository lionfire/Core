using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive;

public static class ObservableEx2
{
    public static IObservable<IChangeSet<TObject, TKey>> CreateConnectOnDemand<TObject, TKey>(Func<TObject, TKey> keySelector,
          Func<SourceCache<TObject, TKey>, IDisposable> resourceFactory,
          Func<TObject, bool>? predicate = null,
          bool suppressEmptyChangeSets = true)
          where TKey : notnull
          where TObject : notnull
    {
        var sourceCache = new SourceCache<TObject, TKey>(keySelector);

        return Observable.Using(() => resourceFactory(sourceCache),
                    _ => sourceCache.Connect(predicate, suppressEmptyChangeSets)
                ).RefCount();
    }
}
