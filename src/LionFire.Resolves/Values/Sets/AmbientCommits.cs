using System.Threading.Tasks;
using LionFire.ExtensionMethods.Poco.Data.Async;
using LionFire.Results;

namespace LionFire.Data.Async.Gets
{
    public class AmbientCommits<TKey, TValue> : Commits<TKey, TValue>
         where TKey : class
        where TValue : class
    {
        public override Task<ISuccessResult> CommitImpl(TValue value) => this.Key.Commit(value);
        //foreach (var service in DependencyContext.Current.GetServices<ICommitter<TKey, TValue>>())
        //{
        //    var result = await service.Put(Key, value).ConfigureAwait(false);
        //    if (result.IsSuccess().HasValue)
        //    {
        //        return result;
        //    }
        //}
    }
}

