using LionFire.Dependencies;
using LionFire.Resolvers;
using System;
using System.Threading.Tasks;
using LionFire.ExtensionMethods;
using LionFire.Results;

namespace LionFire.Data.Async.Gets
{
    public class DynamicCommits<TKey, TValue> : Commits<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        public Func<TKey, TValue, Task<ISuccessResult>> Committer { get; set; }

        public override Task<ISuccessResult> CommitImpl(TValue value) => Committer(Key, value);
    }
}

