using System.Threading.Tasks;
using LionFire.ExtensionMethods.Poco.Data.Async;
using LionFire.Persistence;
using LionFire.Results;

namespace LionFire.Data.Async.Sets;

public class AmbientCommits<TKey, TValue> : Commits<TKey, TValue>
     where TKey : class
    where TValue : class
{
    public override Task<IPersistenceResult> CommitImpl(TValue value) => this.Key.Set(value);
    //foreach (var service in DependencyContext.Current.GetServices<ISetter<TKey, TValue>>())
    //{
    //    var result = await service.Put(Key, value).ConfigureAwait(false);
    //    if (result.IsSuccess().HasValue)
    //    {
    //        return result;
    //    }
    //}
}

