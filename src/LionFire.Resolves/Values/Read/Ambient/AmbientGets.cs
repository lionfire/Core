using LionFire.ExtensionMethods.Poco.Resolvables;
using MorseCode.ITask;

namespace LionFire.Data.Async.Gets
{
    public class AmbientGets<TKey, TValue> : Resolves<TKey, TValue>
         where TKey : class
        where TValue : class
    {
        protected override ITask<IResolveResult<TValue>> ResolveImpl() => this.Key.Resolve<TKey, TValue>().AsITask();
    }
}

