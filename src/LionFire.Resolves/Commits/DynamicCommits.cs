using LionFire.DependencyInjection;
using LionFire.Resolvers;
using System;
using System.Threading.Tasks;
using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Poco.Resolvables;

namespace LionFire.Resolves
{
    public class DynamicCommits<TKey, TValue> : Commits<TKey, TValue>
    {
        public Func<TKey, TValue, Task<IPutResult>> Committer { get; set; }

        public override Task<IPutResult> CommitImpl(TValue value) => Committer(Key, value);
    }

    public class AmbientCommits<TKey, TValue> : Commits<TKey, TValue>
    {
        public override Task<IPutResult> CommitImpl(TValue value) => this.Key.Commit(value);
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

